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
        CameraWebService _cameraService { get; set; }

        [Inject]
        SectorWebService _sectorService { get; set; }

        [Inject]
        FeatureWebService _featureWebService { get; set; }

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

        protected override void OnInitialized()
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;

            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _roiJs = await JS.InvokeAsync<IJSObjectReference>(
                    "import", "./js/roi.js");

                await _roiJs.InvokeVoidAsync("initRoiCanvas");
                await _roiJs.InvokeVoidAsync("syncCanvasWithVideo", "camera-player");
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
