using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Components.CameraComponents;
using GodsEye.WEB.Components.EnvironmentMonitoringComponent;
using GodsEye.WEB.Components.PersonComponents;
using MudBlazor;

namespace GodsEye.WEB.Services
{
    public class DialogWebService
    {
        private readonly IDialogService _dialogService;
        private readonly DialogOptions _options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False, NoHeader = true };

        public DialogWebService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task<IDialogReference?> OpenPersonInfoDialog(int personId)
        {
            var parameters = new DialogParameters<InfoPersonComponent> { { x => x.Id, personId } };
            return await _dialogService.ShowAsync<InfoPersonComponent>(null, parameters, _options);
        }

        public async Task<IDialogReference?> OpenEnvironmentMonitoringSectorDialog(List<EnvironmentMonitoringModel> logs)
        {
            var parameters = new DialogParameters<EnvironmentMonitoringSectorComponent> { { x => x.EnvironmentMonitoringLog,  logs } };
            return await _dialogService.ShowAsync<EnvironmentMonitoringSectorComponent>(null, parameters, _options);
        }

        public async Task OpenCameraData(int cameraId, IMudDialogInstance? mudDialog)
        {
            if (mudDialog is not null)
                mudDialog.CancelAll();

            var parameters = new DialogParameters<CameraDataComponent> { { x => x.Id, cameraId } };
            await _dialogService.ShowAsync<CameraDataComponent>(null, parameters, _options);
        }

        public async Task OpenEnvironmentMonitoring(int cameraId, IMudDialogInstance? mudDialog)
        {
            if (mudDialog is not null)
                mudDialog.CancelAll();

            var parameters = new DialogParameters<CameraEnvironmentMonitoringComponent> { { x => x.Id, cameraId } };
            await _dialogService.ShowAsync<CameraEnvironmentMonitoringComponent>(null, parameters, _options);
        }

        public async Task OpenIncidentRecording(int cameraId, IMudDialogInstance? mudDialog)
        {
            if (mudDialog is not null)
                mudDialog.CancelAll();

            var parameters = new DialogParameters<CameraIncidentRecordingComponent> { { x => x.Id, cameraId } };
            await _dialogService.ShowAsync<CameraIncidentRecordingComponent>(null, parameters, _options);
        }

        public async Task OpenCameraRecognition(int cameraId, IMudDialogInstance? mudDialog)
        {
            if (mudDialog is not null)
                mudDialog.CancelAll();

            var parameters = new DialogParameters<CameraRoiComponent> { { x => x.Id, cameraId } };
            await _dialogService.ShowAsync<CameraRoiComponent>(null, parameters, _options);
        }

        public async Task OpenDwellTimeMonitoring(int cameraId, IMudDialogInstance? mudDialog)
        {
            if (mudDialog is not null)
                mudDialog.CancelAll();

            var parameters = new DialogParameters<CameraDwellTimeRecordingComponent> { { x => x.Id, cameraId } };
            await _dialogService.ShowAsync<CameraDwellTimeRecordingComponent>(null, parameters, _options);
        }

    }
}
