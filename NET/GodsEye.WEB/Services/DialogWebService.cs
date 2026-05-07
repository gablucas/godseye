
using GodsEye.Shared.Response.EnvironmentMonitoring;
using GodsEye.Shared.Response.IncidentRecording;
using GodsEye.WEB.Components.AccessLevelComponents;
using GodsEye.WEB.Components.AccessSchedule;
using GodsEye.WEB.Components.CameraComponents;
using GodsEye.WEB.Components.Compliance;
using GodsEye.WEB.Components.EnvironmentMonitoringComponent;
using GodsEye.WEB.Components.IncidentRecordingComponents;
using GodsEye.WEB.Components.NotificationGroupsComponents;
using GodsEye.WEB.Components.PersonComponents;
using GodsEye.WEB.Components.SectorComponents;
using MudBlazor;
using MudBlazor.Extensions;

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

        #region PERSON DIALOGS
        public async Task OpenCreatePerson(Func<int, Task>? callback = null)
        {
            
            var dialog = await _dialogService.ShowAsync<CreatePersonComponent>("Criar pessoa", _options);

            var result = await dialog.Result;

            if (result.Canceled)
                return;

            if (callback is not null)
            {
                var personId = result.Data.As<int>();
                await callback(personId);
            }
        }

        public async Task<IDialogReference?> OpenPersonUpdateDialog(int personId, IMudDialogInstance? mudDialog)
        {
            if (mudDialog is not null)
                mudDialog.CancelAll();

            var parameters = new DialogParameters<UpdatePersonComponent> { { x => x.PersonId, personId } };
            return await _dialogService.ShowAsync<UpdatePersonComponent>(null, parameters, _options);
        }

        public async Task OpenCreateRecognizePerson(int personId, IMudDialogInstance? mudDialog)
        {
            if (mudDialog is not null)
                mudDialog.CancelAll();

            var parameters = new DialogParameters<UpdatePersonComponent> { { x => x.PersonId, personId } };
            var dialog = await _dialogService.ShowAsync<RecognizePersonComponent>(null, parameters, _options);

            var result = await dialog.Result;

            if (result.Canceled)
                return;
        }

        #endregion

        #region CAMERA DIALOGS
        public async Task OpenCameraData(int cameraId, IMudDialogInstance? mudDialog)
        {
            if (mudDialog is not null)
                mudDialog.CancelAll();

            var parameters = new DialogParameters<CameraDataComponent> { { x => x.Id, cameraId } };
            await _dialogService.ShowAsync<CameraDataComponent>(null, parameters, _options);
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

        #endregion

        public async Task<IDialogReference?> OpenEnvironmentMonitoringSectorDialog(List<EnvironmentMonitoringResponse> logs)
        {
            var parameters = new DialogParameters<EnvironmentMonitoringSectorComponent> { { x => x.EnvironmentMonitoringLog, logs } };
            return await _dialogService.ShowAsync<EnvironmentMonitoringSectorComponent>(null, parameters, _options);
        }

        public async Task OpenIncidentRecording(IncidentRecordingResponse incidentRecording)
        {
            var parameters = new DialogParameters<InfoIncidentRecordingComponent> { { x => x.IncidentRecording, incidentRecording } };

            await _dialogService.ShowAsync<InfoIncidentRecordingComponent>("Registro de incidentes", parameters, _options);
        }

        public async Task OpenCreateNotificationGroup(Func<int, Task>? callback = null)
        {
            var dialog = await _dialogService.ShowAsync<CreateNotificationGroupComponent>("Criar grupo email", _options);

            var result = await dialog.Result;

            if (result.Canceled)
                return;

            if (callback is not null)
            {
                var emailGroupId = result.Data.As<int>();
                await callback(emailGroupId);
            }
        }

        public async Task OpenCreateSector(int? parentId = null, Func<int, Task>? callback = null)
        {
            var parameters = new DialogParameters<CreateSectorComponent> { { x => x.ParentId, parentId } };
            var dialog = await _dialogService.ShowAsync<CreateSectorComponent>("Criar setor", parameters, _options);

            var result = await dialog.Result;

            if (result.Canceled)
                return;

            if (callback is not null)
            {
                var sectorId = result.Data.As<int>();
                await callback(sectorId);
            }
        }

        public async Task OpenHandleRoutine(int? routineId = null, Func<int, Task>? callback = null)
        {
            var parameters = new DialogParameters<UpsertPolicyComponent>();

            if (routineId.HasValue)
            {
                parameters.Add(x => x.Id, routineId.Value);
            }

            var dialog = await _dialogService.ShowAsync<UpsertPolicyComponent>(routineId is null ? "Criar rotina" : "Atualizar rotina", parameters, _options);

            var result = await dialog.Result;

            if (result.Canceled)
                return;

            if (callback is not null)
            {
                var sectorId = result.Data.As<int>();
                await callback(sectorId);
            }
        }


        public async Task OpenCreateAcessLevel(Func<int, Task>? callback = null)
        {
            var dialog = await _dialogService.ShowAsync<CreateAccessLevelComponent>("Criar nível de acesso", _options);

            var result = await dialog.Result;

            if (result.Canceled)
                return;

            if (callback is not null)
            {
                var personId = result.Data.As<int>();
                await callback(personId);
            }
        }

        public async Task OpenCreateAccessSchedule(Func<int, Task>? callback = null)
        {
            var dialog = await _dialogService.ShowAsync<CreateAccessScheduleComponent>("Criar calendário de acesso", _options);

            var result = await dialog.Result;

            if (result.Canceled)
                return;

            if (callback is not null)
            {
                var personId = result.Data.As<int>();
                await callback(personId);
            }
        }

        public async Task OpenCreateCompliance(int? complianceId = null, Func<int, Task>? callback = null)
        {
            var parameters = new DialogParameters<UpsertPolicyComponent>();

            if (complianceId.HasValue)
            {
                parameters.Add(x => x.Id, complianceId.Value);
            }

            var dialog = await _dialogService.ShowAsync<UpsertPolicyComponent>(complianceId is null ? "Criar política" : "Atualizar política", parameters, _options);

            var result = await dialog.Result;

            if (result.Canceled)
                return;

            if (callback is not null)
            {
                var sectorId = result.Data.As<int>();
                await callback(sectorId);
            }
        }


    }
}
