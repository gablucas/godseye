using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.WEB.Enum;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace GodsEye.WEB.Pages.Camera
{
    public partial class CameraRecognitionPage
    {
        #region DI

        [Inject]
        CameraWebService _cameraService { get; set; }

        [Inject]
        SectorService _sectorService { get; set; }

        [Inject]
        FeatureWebService _featureWebService { get; set; }

        [Inject]
        IJSRuntime JS { get; set; }

        #endregion

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

        //protected override async Task OnParametersSetAsync()
        //{

        //    var result = await _cameraService.GetById(Id);

        //    if (result.Success && result is not null && result.Data is not null)
        //    {
        //        camera = result.Data;

        //        RecognitionForm = new CreateCameraRecognitionForm()
        //        {
        //            Id = camera.Id,
        //            Name = camera.Name,
        //            Connection = camera.Connection,
        //            Features = camera.Features.Select(x => x.FeatureId),
        //            SectorId = camera.SectorId.ToString()
        //        };
        //    }

        //    base.OnParametersSet();
        //}

        protected override void OnInitialized()
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;

            base.OnInitialized();
        }

        private async Task Submit()
        {
            //if (!ValidateFeatures())
            //    return;

            //visible = true;
            //apiResponse = await _cameraService.UpdateAsync(CameraForm);
            //visible = false;

            //if (apiResponse.Success)
            //{
            //    Snackbar.Add("Camera atualizada com sucesso!", Severity.Success);
            //    success = false;

            //    var result = await _cameraService.GetById(camera.Id);

            //    if (result.Success && result is not null && result.Data is not null)
            //    {
            //        camera = result.Data;
            //    }

            //    _refreshToken = Guid.NewGuid();
            //}
            //else
            //{
            //    Snackbar.Add("Houve um erro ao cadastrar a camera, tente novamente mais tarde", Severity.Error);
            //}
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _roiJs = await JS.InvokeAsync<IJSObjectReference>(
                    "import", "./js/roi.js");

                await _roiJs.InvokeVoidAsync("initRoiCanvas");
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
    }
}

