using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace GodsEye.WEB.Components.CameraComponents
{
    public partial class CameraDataComponent
    {
        #region DI

        [Inject]
        CameraWebService CameraWebService { get; set; }

        [Inject]
        SectorWebService SectorWebService { get; set; }

        [Inject]
        FeatureWebService FeatureWebService { get; set; }

        [Inject]
        MediaMtxWebService MediaMtxWebService { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        #endregion

        #region PARAMS

        [Parameter]
        public int Id { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        #endregion

        #region FORM

        MudForm form;
        UpdateCameraForm CameraForm { get; set; } = new();
        IEnumerable<SectorModel> _sectors = Enumerable.Empty<SectorModel>();
        IEnumerable<FeatureModel> _features = Enumerable.Empty<FeatureModel>();

        private bool success;
        private string[] errors = { };

        private bool _loadingConnection = false;
        private bool _hasConnectionError = false;
        private string? _connectionErrorMessage = null;

        private bool? _mediaMtxStatus = null;

        #endregion

        public CameraModel camera { get; set; }

        ApiResponse<ProcedureResult?>? apiResponse { get; set; } = null;

        private bool visible = false;

        Guid _refreshToken = Guid.NewGuid();

        protected override async Task OnParametersSetAsync()
        {

            var result = await CameraWebService.GetById(Id);

            if (result.Success && result is not null && result.Data is not null)
            {
                camera = result.Data;

                CameraForm = new UpdateCameraForm()
                {
                    Id = camera.Id,
                    Name = camera.Name,
                    Connection = camera.Connection,
                    Features = camera.Features.Select(x => x.FeatureId),
                    SectorId = camera.SectorId.ToString()
                };

                _ = StartStream();
            }

            base.OnParametersSet();
        }

        protected override void OnInitialized()
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;

            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            var response = await SectorWebService.GetAllAsync();
            if (response is not null && response.Success)
            {
                _sectors = response.Data;
            }

            var featureResponse = await FeatureWebService.GetAllAsync();
            if (featureResponse is not null && featureResponse.Success)
            {
                _features = featureResponse.Data;
            }
        }

        private void OnFeatureToggled(int featureId, bool isChecked)
        {

            var list = CameraForm.Features.ToList();

            if (isChecked)
            {
                if (!list.Contains(featureId))
                    list.Add(featureId);
            }
            else
            {
                list.Remove(featureId);
            }

            CameraForm.Features = list;

            //ValidateFeatures();
        }

        public async Task StartStream()
        {
            _loadingConnection = true;

            if (string.IsNullOrEmpty(CameraForm.Connection))
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


            var cam = await MediaMtxWebService.StartStream(CameraForm.Connection);

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
            visible = true;
            apiResponse = await CameraWebService.UpdateAsync(CameraForm);
            visible = false;

            if (apiResponse.Success)
            {
                Snackbar.Add("Camera atualizada com sucesso!", Severity.Success);
                //MudDialog.Close(DialogResult.Ok(1));
            }
            else
            {
                Snackbar.Add("Houve um erro ao cadastrar o setor, tente novamente mais tarde", Severity.Error);
            }

        }

        private void Cancel() => MudDialog.Cancel();
    }
}
