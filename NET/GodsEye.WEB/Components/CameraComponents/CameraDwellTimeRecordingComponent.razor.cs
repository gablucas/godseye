using GodsEye.Application.UseCases.Camera.Commands.CreateCameraConfigDwellTimeMonitoring;
using GodsEye.Application.UseCases.Camera.Commands.UpdateCameraConfigDwellTimeMonitoring;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.CameraComponents
{
    public partial class CameraDwellTimeRecordingComponent
    {
        [Inject]
        public CameraWebService CameraWebService { get; set; }

        [Parameter]
        public int Id { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        #region FORM

        MudForm form;
        CameraDwellTimeMonitoringForm DwellTimeMonitoringForm { get; set; } = new();
        private bool success;
        private string[] errors = { };

        #endregion

        private bool visible = false;

        protected override async Task OnParametersSetAsync()
        {
            var cameraConfigDwellTimeResponse = await CameraWebService.GetConfigDwellTimeMonitoring(Id);

            if (cameraConfigDwellTimeResponse is null || !cameraConfigDwellTimeResponse.Success)
            {
                Snackbar.Add("Houve um erro ao buscar os dados de configuração", Severity.Error);
                return;
            }

            DwellTimeMonitoringForm = new CameraDwellTimeMonitoringForm()
            {
                Id = cameraConfigDwellTimeResponse.Data.Id,
                CameraId = cameraConfigDwellTimeResponse.Data.CameraId,
                MaxDwellTimeMinutes = cameraConfigDwellTimeResponse.Data.MaxDwellTimeMinutes,
                MaxNonIdentificationTimeMinutes = cameraConfigDwellTimeResponse.Data.MaxNonIdentificationTimeMinutes
            };

        }

        private async Task Submit()
        {
            if (DwellTimeMonitoringForm.Id == 0)
            {
                var createRequest = new CreateCameraConfigDwellTimeMonitoringRequest(Id, DwellTimeMonitoringForm.MaxDwellTimeMinutes, DwellTimeMonitoringForm.MaxNonIdentificationTimeMinutes);
                var createResult = await CameraWebService.CreateConfigDwellTimeMonitoring(createRequest);

                if (createResult.Success)
                {
                    Snackbar.Add("Configuração salva com sucesso.", Severity.Success);
                    DwellTimeMonitoringForm.Id = createResult.Data;
                }
                else
                {
                    Snackbar.Add("Houve um erro ao criar a configuração.", Severity.Error);
                }

            }
            else
            {
                var updateRequest = new UpdateCameraConfigDwellTimeMonitoringRequest(DwellTimeMonitoringForm.Id, DwellTimeMonitoringForm.MaxDwellTimeMinutes, DwellTimeMonitoringForm.MaxNonIdentificationTimeMinutes);
                var updateResult = await CameraWebService.UpdateConfigDwellTimeMonitoring(updateRequest);

                if (updateResult.Success)
                {
                    Snackbar.Add("Configuração atualizada com sucesso.", Severity.Success);
                    DwellTimeMonitoringForm.Id = updateResult.Data;
                }
                else
                {
                    Snackbar.Add("Houve um erro ao atualizar a configuração.", Severity.Error);
                }
            }
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
