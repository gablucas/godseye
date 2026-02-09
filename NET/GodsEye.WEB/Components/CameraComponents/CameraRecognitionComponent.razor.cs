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

                await _roiJs.InvokeVoidAsync("initRoiCanvas");
                await _roiJs.InvokeVoidAsync("syncCanvasWithVideo", "camera-player");
                await ClearStrokeRect();

                var cameraRequest = await CameraService.GetById(Id);

                if (cameraRequest.Success)
                {
                    camera = cameraRequest.Data;
                    _ = StartStream();
                }
            }
        }

        async Task GetRect()
        {
            var rect = await _roiJs.InvokeAsync<Rect>("getRect");

            if (rect == null)
                return;

            if (_recognizeType == RecognizeRectEnum.Face)
            {
                RecognitionForm.FaceDimension.Width = rect.Width;
                RecognitionForm.FaceDimension.Height = rect.Height;
                RecognitionForm.FaceDimension.X = rect.X;
                RecognitionForm.FaceDimension.Y = rect.Y;
            }
            else if (_recognizeType == RecognizeRectEnum.Camera)
            {
                RecognitionForm.CameraDimension.Width = rect.Width;
                RecognitionForm.CameraDimension.Height = rect.Height;
                RecognitionForm.CameraDimension.X = rect.X;
                RecognitionForm.CameraDimension.Y = rect.Y;
            }
        }

        async Task SetStrokeRect(RecognizeRectEnum recognizeType)
        {
            _recognizeType = null;

            if (recognizeType == RecognizeRectEnum.Face)
            {
                await _roiJs.InvokeVoidAsync(
                    "setStrokeRect",
                    RecognitionForm.FaceDimension
                );
            }
            else if (recognizeType == RecognizeRectEnum.Camera)
            {
                await _roiJs.InvokeVoidAsync(
                    "setStrokeRect",
                    RecognitionForm.CameraDimension
                );
            }
        }

        async Task ClearStrokeRect()
        {
            await _roiJs.InvokeVoidAsync("clearStrokeRect");
        }

        async Task ActiveDrawing(RecognizeRectEnum recognizeType)
        {
            _recognizeType = recognizeType;
            await _roiJs.InvokeVoidAsync("enableDrawing");
        }

        async Task DesactiveDrawing()
        {
            _recognizeType = null;
            await _roiJs.InvokeVoidAsync("disableDrawing");
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
