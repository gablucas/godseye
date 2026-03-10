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
    public partial class CreateCameraComponent
    {
        #region DI

        [Inject]
        CameraWebService CameraService { get; set; }

        [Inject]
        SectorWebService SectorService { get; set; }

        [Inject]
        FeatureWebService FeatureService { get; set; }

        [Inject]
        MediaMtxWebService MediaMtxWebService { get; set; }

        [Inject]
        DialogWebService DialogWebService { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        #endregion

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public int Id { get; set; }

        #region FORM

        MudForm form;
        private bool success;
        private string[] errors = { };
        private string featureError;
        public CreateCameraForm CameraForm { get; set; } = new();
        IEnumerable<SectorModel> _sectors = Enumerable.Empty<SectorModel>();
        IEnumerable<FeatureModel> _features = Enumerable.Empty<FeatureModel>();

        private bool _hasConnectionError = false;
        private string? _connectionErrorMessage = null;
        private bool _loadingConnection = false;

        ApiResponse<int>? apiResponse { get; set; } = null;

        private bool visible = false;
        private bool _videoExpanded = false;

        #endregion


        protected override async Task OnInitializedAsync()
        {
            var response = await SectorService.GetAllAsync();
            if (response is not null && response.Success)
            {
                _sectors = response.Data;
            }

            var featureResponse = await FeatureService.GetAllAsync();
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

            ValidateFeatures();
        }

        private bool ValidateFeatures()
        {
            if (CameraForm.Features == null || !CameraForm.Features.Any())
            {
                featureError = "Selecione pelo menos uma funcionalidade";
                success = false;
            }

            featureError = null;
            return true;
        }

        private void BackToRegister()
        {
            apiResponse = null;
        }

        private async Task Submit()
        {
            if (!ValidateFeatures())
                return;

            visible = true;
            apiResponse = await CameraService.CreateAsync(CameraForm);
            visible = false;

            if (apiResponse.Success)
            {
                Snackbar.Add("camera cadastrada com Success!", Severity.Success);
                MudDialog.Close(DialogResult.Ok(apiResponse.Data));
            }
            else
            {
                Snackbar.Add("Houve um erro ao cadastrar a camera, tente novamente mais tarde", Severity.Error);
            }   
        }

        private void Cancel() => MudDialog.Cancel();

        private void OnConnectionChanged(string value)
        {
            CameraForm.Connection = value;
            _hasConnectionError = false;
            _connectionErrorMessage = null;
        }


        public async Task StartStream()
        {
            _loadingConnection = true;

            if (string.IsNullOrEmpty(CameraForm.Connection))
            {
                _loadingConnection = false;
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
            _videoExpanded = true;

            await JS.InvokeVoidAsync("streamFunctions.start", "camera-player", webRtcUrl);

            _loadingConnection = false;
            _hasConnectionError = false;
            _connectionErrorMessage = null;
        }

        public async ValueTask DisposeAsync()
        {
            await JS.InvokeVoidAsync("streamFunctions.stop", "camera-player");
        }

        private async Task CreateNewSectorCallback(int sectorId)
        {
            var newSector = await SectorService.GetById(sectorId);

            if (newSector.Success)
            {
                CameraForm.SectorId = sectorId;
                _sectors = _sectors.Append(newSector.Data).ToList();
                StateHasChanged();
            }
        }
    }
}
