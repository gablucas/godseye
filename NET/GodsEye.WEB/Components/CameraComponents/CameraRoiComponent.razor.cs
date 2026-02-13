using GodsEye.Application.DTOs.Model;
using GodsEye.Application.UseCases.Camera.Commands.CreateCameraRoi;
using GodsEye.Application.UseCases.Camera.Commands.UpdateCameraRoi;
using GodsEye.Domain.Enums;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace GodsEye.WEB.Components.CameraComponents
{
    public partial class CameraRoiComponent
    {
        #region DI

        [Inject]
        CameraWebService CameraService { get; set; }

        [Inject]
        MediaMtxWebService MediaMtxWebService { get; set; }

        [Inject]
        IJSRuntime JS { get; set; }

        #endregion

        #region PARAMS

        [Parameter]
        public int Id { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        #endregion

        #region FORM

        MudForm form;
        CameraRoiForm FaceRoi { get; set; } = new();
        CameraRoiForm EnvironmentRoi { get; set; } = new();

        #endregion

        #region VARIABLES

        private IJSObjectReference _roiJs;

        public CameraModel camera { get; set; }


        private bool _loadingConnection = true;
        private bool? _mediaMtxStatus = null;

        private bool _hasConnectionError = false;
        private string? _connectionErrorMessage = null;

        private bool isDrawingActive = false;
        private RoiTypeEnum activeContext = RoiTypeEnum.FaceDetection;

        private DotNetObjectReference<CameraRoiComponent>? _objRef;

        #endregion

        #region LIFETIME FUNCS

        protected override void OnInitialized()
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;

            _objRef = DotNetObjectReference.Create(this);

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var version = DateTime.Now.Ticks;

                _roiJs = await JS.InvokeAsync<IJSObjectReference>(
                    "import", $"./js/roi.js?v={version}");

                await _roiJs.InvokeVoidAsync("initRoiCanvas", "camera-player", "roiCanvas", _objRef);

                var cameraRequest = await CameraService.GetById(Id);

                if (cameraRequest.Success)
                {
                    camera = cameraRequest.Data;
                    await StartStream();
                    await GetCameraRoi();
                }

                await _roiJs.InvokeVoidAsync("renderExistingShape", FaceRoi.Coordinates, "rect");
            }
        }

        #endregion

        public async ValueTask DisposeAsync()
        {
            await JS.InvokeVoidAsync("streamFunctions.stop", "camera-player");

            // Libere a memória da referência
            _objRef?.Dispose();
        }

        #region DRAW FUNCS

        [JSInvokable]
        public async Task OnVideoReady()
        {
            Console.WriteLine("Vídeo carregado! Sincronizando canvas...");

            await _roiJs.InvokeVoidAsync("resizeCanvasToVideo");

            // PRECAUÇÃO: Verifica se as coordenadas já existem (caso o vídeo carregue antes da API)
            // Se FaceRoi.Id for 0, talvez a API ainda não tenha retornado.
            // Mas se você inicializa com 'new()', o objeto não é null, então o código não quebra,
            // apenas desenha nada (o que é aceitável).

            // Apenas garanta que o Coordinates não seja nulo antes de enviar
            if (activeContext == RoiTypeEnum.FaceDetection && FaceRoi?.Coordinates != null)
            {
                await _roiJs.InvokeVoidAsync("renderExistingShape", FaceRoi.Coordinates, "rect");
            }
            else if (EnvironmentRoi?.Coordinates != null)
            {
                await _roiJs.InvokeVoidAsync("renderExistingShape", EnvironmentRoi.Coordinates, "polygon");
            }

            StateHasChanged();
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

        private async Task StartDrawingFace(RoiTypeEnum roiType)
        {
            isDrawingActive = true;
            
            if (roiType == RoiTypeEnum.FaceDetection)
            {
                await _roiJs.InvokeVoidAsync("startDrawing", "rect");
            }
            else if (roiType == RoiTypeEnum.RestrictedArea)
            {
                await _roiJs.InvokeVoidAsync("startDrawing", "polygon");
            }
        }

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

                    Snackbar.Add("Dimensão da face definida.", Severity.Info);
                }
                else if (activeContext == RoiTypeEnum.RestrictedArea)
                {
                    // CORREÇÃO 3: Atribua o objeto inteiro
                    EnvironmentRoi.Coordinates = shapeData;

                    Snackbar.Add("Polígono de área definido.", Severity.Info);
                }
            }

            await _roiJs.InvokeVoidAsync("stopDrawing");
            isDrawingActive = false;
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
                await _roiJs.InvokeVoidAsync("renderExistingShape", EnvironmentRoi.Coordinates, "polygon");

        }

        private async Task ResetInteraction()
        {
            isDrawingActive = false;
            await _roiJs.InvokeVoidAsync("clearCanvas");
        }

        #endregion

        #region STREAM FUNCS

        public async Task StartStream()
        {
            _loadingConnection = true;

            if (string.IsNullOrEmpty(camera.Connection))
            {
                _loadingConnection = false;
                return;
            }

            var isMediaMtxOnline = await MediaMtxWebService.CheckStatus();

            if (!isMediaMtxOnline.Success || !isMediaMtxOnline.Data)
            {
                _mediaMtxStatus = false;
                _loadingConnection = false;
                await InvokeAsync(StateHasChanged);
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

        #endregion

        #region REQUEST FUNCS

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

        private async Task Delete(RoiTypeEnum roiType)
        {
            CameraRoiForm? roiForm = roiType switch
            {
                RoiTypeEnum.FaceDetection => FaceRoi,
                RoiTypeEnum.RestrictedArea => EnvironmentRoi,
                _ => null
            };

            if (roiForm is null || roiForm.Id <= 0)
                return;

            var id = roiForm.Id;

            // (Opcional) confirmação do usuário
            // var confirmar = await DialogService.ShowMessageBox("Confirmação", "Excluir área?", yesText: "Sim", cancelText: "Cancelar");
            // if (confirmar != true) return;

            // 2. Chamar API
            var deleteResult = await CameraService.DeleteRoiAsync(id);

            if (deleteResult is null || !deleteResult.Success)
            {
                Snackbar.Add("Erro ao excluir área.", Severity.Error);  
                return;
            }

            await ResetInteraction();

            switch (roiType)
            {
                case RoiTypeEnum.FaceDetection:
                    FaceRoi = new CameraRoiForm();
                    break;

                case RoiTypeEnum.RestrictedArea:
                    EnvironmentRoi = new CameraRoiForm();
                    break;
            }

            Snackbar.Add("Área excluída com sucesso.", Severity.Success);
        }

        private async Task Submit(RoiTypeEnum roiType)
        {
            var selectedCameraRoiForm = new CameraRoiForm();

            switch (roiType)
            {
                case RoiTypeEnum.FaceDetection:
                    selectedCameraRoiForm = FaceRoi;
                    break;
                case RoiTypeEnum.RestrictedArea:
                    selectedCameraRoiForm = EnvironmentRoi;
                    break;
            }

            if (selectedCameraRoiForm.Id == 0)
            {
                var createRequest = new CreateCameraRoiRequest(Id, roiType, selectedCameraRoiForm.Coordinates);
                var createResult = await CameraService.CreateRoiAsync(createRequest);
                Snackbar.Add("Área salva com sucesso.", Severity.Success);
            }
            else
            {
                var updateRequest = new UpdateCameraRoiRequest(selectedCameraRoiForm.Id, selectedCameraRoiForm.Coordinates);
                var updateResult = await CameraService.UpdateRoiAsync(updateRequest);
                Snackbar.Add("Área atualizada com sucesso.", Severity.Success);
            }
        }

        #endregion[

        private void Cancel() => MudDialog.Cancel();
    }
}
