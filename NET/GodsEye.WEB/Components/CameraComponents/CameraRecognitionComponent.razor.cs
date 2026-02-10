using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.UseCases.Camera.Commands.CreateCameraRoi;
using GodsEye.Application.UseCases.Camera.Commands.UpdateCameraRoi;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Enums;
using GodsEye.WEB.Enum;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace GodsEye.WEB.Components.CameraComponents
{
    public partial class CameraRecognitionComponent
    {
        #region DI

        [Inject]
        CameraWebService CameraService { get; set; }

        [Inject]
        SectorWebService SectorService { get; set; }

        [Inject]
        FeatureWebService FeatureWebService { get; set; }

        [Inject]
        MediaMtxWebService MediaMtxWebService { get; set; }

        [Inject]
        IJSRuntime JS { get; set; }

        #endregion

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        #region PARAMS

        [Parameter]
        public int Id { get; set; }

        #endregion

        #region FORM

        MudForm form;
        CameraRoiForm FaceRoi { get; set; } = new();
        CameraRoiForm EnvironmentRoi { get; set; } = new();

        private bool success;
        private string[] errors = { };
        private string featureError;

        #endregion

        private IJSObjectReference _roiJs;

        public CameraModel camera { get; set; }

        ApiResponse<ProcedureResult?>? apiResponse { get; set; } = null;

        private bool visible = false;

        private bool _loadingConnection = true;
        private bool _hasConnectionError = false;
        private string? _connectionErrorMessage = null;

        private bool isDrawingActive = false;
        private string drawingMode = ""; // "rect" ou "polygon"
        private RoiTypeEnum activeContext = RoiTypeEnum.FaceDetection; // Face ou Camera
        private int activeTabIndex = 0;

        MudForm faceForm;
        MudForm areaForm;

        bool faceSuccess;
        string[] faceErrors = { };

        bool areaSuccess;
        string[] areaErrors = { };

        protected override void OnInitialized()
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var version = DateTime.Now.Ticks;

                _roiJs = await JS.InvokeAsync<IJSObjectReference>(
                    "import", $"./js/roi.js?v={version}");

                await _roiJs.InvokeVoidAsync("initRoiCanvas", "camera-player", "roiCanvas");

                var cameraRequest = await CameraService.GetById(Id);

                if (cameraRequest.Success)
                {
                    camera = cameraRequest.Data;
                    _ = StartStream();
                    _ = GetCameraRoi();
                }
            }
        }

        private async Task SelectTab(RoiTypeEnum context)
        {
            // Limpa estado anterior
            await _roiJs.InvokeVoidAsync("stopDrawing");
            await _roiJs.InvokeVoidAsync("clearCanvas");

            isDrawingActive = false;
            activeContext = context;

            // 2. SEGUNDO: Carrega o desenho salvo (se houver)
            if (context == RoiTypeEnum.FaceDetection)
            {
                // Valida se tem dimensões E se tem o ponto de origem salvo
                if (FaceRoi.Coordinates.Width > 0 &&
                    FaceRoi.Coordinates.Points != null &&
                    FaceRoi.Coordinates.Points.Any())
                {
                    await _roiJs.InvokeVoidAsync("renderExistingShape", FaceRoi.Coordinates, "rect");
                }
            }
            else if (context == RoiTypeEnum.RestrictedArea)
            {
                if (EnvironmentRoi.Coordinates.Points != null &&
                    EnvironmentRoi.Coordinates.Points.Count() > 1) // Polígono precisa de > 1 ponto
                {
                    await _roiJs.InvokeVoidAsync("renderExistingShape", EnvironmentRoi.Coordinates, "polygon");
                }
            }
        }

        private async Task StartDrawingFace()
        {
            isDrawingActive = true;
            drawingMode = "rect";
            await _roiJs.InvokeVoidAsync("startDrawing", "rect");
        }

        // Ação: Iniciar Desenho da Área (Polígono Apenas)
        private async Task StartDrawingArea()
        {
            isDrawingActive = true;
            drawingMode = "polygon";
            await _roiJs.InvokeVoidAsync("startDrawing", "polygon");
        }

        // Ação: Confirmar e Salvar no Objeto C#
        private async Task ConfirmDrawing()
        {
            // O JS retorna um objeto completo com Width, Height e Points
            var shapeData = await _roiJs.InvokeAsync<RoiModel>("getShapeData");

            if (shapeData != null)
            {
                if (activeContext == RoiTypeEnum.FaceDetection)
                {
                    // CORREÇÃO 1: Não limpe a lista! 
                    // O JS retorna uma lista com 1 item para Face (que é o X, Y do canto).
                    // Se você der new List(), perde a posição do retângulo.

                    // CORREÇÃO 2: Atribua o objeto inteiro, não a propriedade Points
                    FaceRoi.Coordinates = shapeData;

                    Snackbar.Add("Dimensão da face definida.", Severity.Success);
                }
                else if (activeContext == RoiTypeEnum.RestrictedArea)
                {
                    // CORREÇÃO 3: Atribua o objeto inteiro
                    EnvironmentRoi.Coordinates = shapeData;

                    Snackbar.Add("Polígono de área definido.", Severity.Success);
                }
            }

            await _roiJs.InvokeVoidAsync("stopDrawing");
            isDrawingActive = false;
            // Opcional: Força renderizar de novo para garantir que o visual reflete o objeto salvo
            StateHasChanged();
        }

        private async Task UndoLastPoint()
        {
            await _roiJs.InvokeVoidAsync("undo");
        }

        private async Task CancelDrawing(RoiTypeEnum roiType)
        {
            isDrawingActive = false;

            if (roiType == RoiTypeEnum.FaceDetection)
                await _roiJs.InvokeVoidAsync("renderExistingShape", FaceRoi.Coordinates, "rect");

            else if(roiType == RoiTypeEnum.RestrictedArea)
                await _roiJs.InvokeVoidAsync("renderExistingShape", EnvironmentRoi.Coordinates, "rect");

        }

        private async Task ResetInteraction()
        {
            isDrawingActive = false;
            await _roiJs.InvokeVoidAsync("clearCanvas");
        }

        public async Task StartStream()
        {
            _loadingConnection = true;

            if (string.IsNullOrEmpty(camera.Connection))
            {
                _loadingConnection = false;
                return;
            }


            var cam = await MediaMtxWebService.StartStream(camera.Connection);

            if (cam is null || !cam.Success)
            {
                _hasConnectionError = true;
                _connectionErrorMessage = cam.Error.Message;
                _loadingConnection = false;
                return;
            }

            var webRtcUrl = cam.Data;

            await JS.InvokeVoidAsync("streamFunctions.start", "camera-player", webRtcUrl);

            _loadingConnection = false;
            _hasConnectionError = false;
            _connectionErrorMessage = null;
        }

        public async Task GetCameraRoi()
        {
            var getCameraRoi = await CameraService.GetRoiByCameraId(Id);

            if (getCameraRoi == null || !getCameraRoi.Success)
                return;

            foreach (var cameraRoi in getCameraRoi.Data)
            {
                if (cameraRoi.RoiType == RoiTypeEnum.FaceDetection)
                {
                    FaceRoi = new CameraRoiForm
                    {
                        Id = cameraRoi.Id,
                        RoiType = cameraRoi.RoiType,
                        Coordinates = cameraRoi.Coordinates,
                    };
                }
                else
                {
                    EnvironmentRoi = new CameraRoiForm
                    {
                        Id = cameraRoi.Id,
                        RoiType = cameraRoi.RoiType,
                        Coordinates = cameraRoi.Coordinates,
                    };
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            await JS.InvokeVoidAsync("streamFunctions.stop", "camera-player");
        }


        private async Task Delete(CameraRoiForm cameraRoi)
        {
            var deleteResult = await CameraService.DeelteRoiAsync(cameraRoi.Id);
        }

        private async Task Submit(CameraRoiForm cameraRoi, RoiTypeEnum roiType)
        {
            visible = true;

            if (cameraRoi.Id == 0)
            {
                var createRequest = new CreateCameraRoiRequest(Id, roiType, cameraRoi.Coordinates);
                var createResult = await CameraService.CreateRoiAsync(createRequest);
            }
            else
            {
                var updateRequest = new UpdateCameraRoiRequest(cameraRoi.Id, cameraRoi.Coordinates);
                var updateResult = await CameraService.UpdateRoiAsync(updateRequest);
            }

            visible = false;

            //if (apiResponse.Success)
            //{
            //    Snackbar.Add("Camera atualizada com sucesso!", Severity.Success);
            //    //MudDialog.Close(DialogResult.Ok(1));
            //}
            //else
            //{
            //    Snackbar.Add("Houve um erro ao cadastrar o setor, tente novamente mais tarde", Severity.Error);
            //}

        }

        private void Cancel() => MudDialog.Cancel();
    }
}
