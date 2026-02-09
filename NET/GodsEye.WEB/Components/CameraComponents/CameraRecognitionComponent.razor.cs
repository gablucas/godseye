using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
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
        UpdateCameraRecognitionForm RecognitionForm { get; set; } = new();

        private bool success;
        private string[] errors = { };
        private string featureError;

        #endregion

        private IJSObjectReference _roiJs;

        public CameraModel camera { get; set; }

        ApiResponse<ProcedureResult?>? apiResponse { get; set; } = null;

        private bool visible = false;

        private RecognizeRectEnum? _recognizeType = null;

        private bool _face = false;
        private bool _area = false;

        private bool _loadingConnection = false;
        private bool _hasConnectionError = false;
        private string? _connectionErrorMessage = null;

        private bool isDrawingActive = false;
        private string drawingMode = ""; // "rect" ou "polygon"
        private RecognizeRectEnum? activeContext = null; // Face ou Camera
        private int activeTabIndex = 0;

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
                }
            }
        }

        private async Task SelectTab(RecognizeRectEnum context)
        {
            // 1. PRIMEIRO: Para qualquer desenho ativo e limpa o canvas
            // Isso evita que o JS tente desenhar um polígono enquanto carrega um retângulo
            await _roiJs.InvokeVoidAsync("stopDrawing");
            await _roiJs.InvokeVoidAsync("clearCanvas");

            isDrawingActive = false;
            activeContext = context;

            // 2. SEGUNDO: Carrega o desenho salvo (se houver)
            if (context == RecognizeRectEnum.Face && RecognitionForm.FaceDimension != null)
            {
                // Se a face tiver width > 0, renderiza
                if (RecognitionForm.FaceDimension.Width > 0)
                {
                    await _roiJs.InvokeVoidAsync("renderExistingShape", RecognitionForm.FaceDimension, "rect");
                }
            }
            else if (context == RecognizeRectEnum.Camera && RecognitionForm.CameraDimension != null)
            {
                // Verifica se é polígono (tem pontos) ou retângulo legado
                bool hasPoints = RecognitionForm.CameraDimension.Points != null && RecognitionForm.CameraDimension.Points.Any();

                if (hasPoints)
                {
                    await _roiJs.InvokeVoidAsync("renderExistingShape", RecognitionForm.CameraDimension, "polygon");
                }
                else if (RecognitionForm.CameraDimension.Width > 0)
                {
                    // Caso legado: era retângulo, renderiza como retângulo mas força modo visual
                    await _roiJs.InvokeVoidAsync("renderExistingShape", RecognitionForm.CameraDimension, "rect");
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
            // Busca dados do JS no formato { x, y, width, height, points: [{x,y}] }
            var shapeData = await _roiJs.InvokeAsync<Rect>("getShapeData");

            if (shapeData != null)
            {
                if (activeContext == RecognizeRectEnum.Face)
                {
                    // Garante que face é apenas retangulo (ignora pontos se vierem por bug)
                    shapeData.Points = new List<Point>();
                    RecognitionForm.FaceDimension = shapeData;
                    Snackbar.Add("Dimensão da face definida.", Severity.Success);
                }
                else if (activeContext == RecognizeRectEnum.Camera)
                {
                    RecognitionForm.CameraDimension = shapeData;
                    Snackbar.Add("Polígono de área definido.", Severity.Success);
                }
            }

            // Desativa modo de desenho no JS e na UI
            await _roiJs.InvokeVoidAsync("stopDrawing");
            isDrawingActive = false;
        }

        private async Task UndoLastPoint()
        {
            await _roiJs.InvokeVoidAsync("undo");
        }

        private async Task CancelDrawing()
        {
            isDrawingActive = false;
            await _roiJs.InvokeVoidAsync("stopDrawing");

            // Opcional: Recarregar o desenho original salvo anteriormente
            if (activeContext != null) await SelectTab(activeContext.Value);
        }

        private async Task ResetInteraction()
        {
            activeContext = null;
            await CancelDrawing();
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

        public async ValueTask DisposeAsync()
        {
            await JS.InvokeVoidAsync("streamFunctions.stop", "camera-player");
        }


        private async Task Submit()
        {
            //visible = true;
            //apiResponse = await _cameraService.UpdateAsync(CameraForm);
            //visible = false;

            //if (apiResponse.Success)
            //{
            //    Snackbar.Add("Camera atualizada com sucesso!", Severity.Success);
            //    MudDialog.Close(DialogResult.Ok(1));
            //}
            //else
            //{
            //    Snackbar.Add("Houve um erro ao cadastrar o setor, tente novamente mais tarde", Severity.Error);
            //}

        }

        private void Cancel() => MudDialog.Cancel();
    }
}
