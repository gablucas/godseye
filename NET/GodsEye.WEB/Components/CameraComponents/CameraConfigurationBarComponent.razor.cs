using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.CameraComponents
{
    public partial class CameraConfigurationBarComponent
    {
        [Inject]
        public CameraWebService cameraService { get; set;  }

        [Parameter]
        public Guid RefreshToken { get; set; }

        [Parameter]
        public string Pagina { get; set; }

        [Parameter]
        public int Id { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        private List<CameraFeatureModel> _cameraFeatures = new();

        protected override async Task OnParametersSetAsync()
        {
            var result = await cameraService.GetFeatures(Id);
            _cameraFeatures = result.ToList();
        }

        private bool HasFeature(int featureId)
        {
            return _cameraFeatures.Any(x => x.Id == featureId);
        }

        
        private async Task OpenCameraData(int cameraId)
        {
            MudDialog.CancelAll();

            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False, NoHeader = true };
            var parameters = new DialogParameters<CameraDataComponent> { { x => x.Id, cameraId } };
            var dialog = await DialogService.ShowAsync<CameraDataComponent>(null, parameters, options);

            var result = await dialog.Result;
        }

        private async Task OpenEnvironmentMonitoring(int cameraId)
        {
            MudDialog.CancelAll();

            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False, NoHeader = true };
            var parameters = new DialogParameters<CameraEnvironmentMonitoringComponent> { { x => x.Id, cameraId } };
            var dialog = await DialogService.ShowAsync<CameraEnvironmentMonitoringComponent>(null, parameters, options);

            var result = await dialog.Result;
        }

        private async Task OpenIncidentRecording(int cameraId)
        {
            MudDialog.CancelAll();

            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False, NoHeader = true };
            var parameters = new DialogParameters<CameraIncidentRecordingComponent> { { x => x.Id, cameraId } };
            var dialog = await DialogService.ShowAsync<CameraIncidentRecordingComponent>(null, parameters, options);

            var result = await dialog.Result;
        }

        private async Task OpenCameraRecognition(int cameraId)
        {
            MudDialog.CancelAll();

            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False, NoHeader = true };
            var parameters = new DialogParameters<CameraRoiComponent> { { x => x.Id, cameraId } };
            await DialogService.ShowAsync<CameraRoiComponent>(null, parameters, options);
        }

        private async Task OpenDwellTimeMonitoring(int cameraId)
        {
            MudDialog.CancelAll();

            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False, NoHeader = true };
            var parameters = new DialogParameters<CameraDwellTimeRecordingComponent> { { x => x.Id, cameraId } };
            await DialogService.ShowAsync<CameraDwellTimeRecordingComponent>(null, parameters, options);
        }
    }
}
