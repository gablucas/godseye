using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.CameraComponents
{
    public partial class CameraIncidentRecordingComponent
    {
        #region DI

        [Inject]
        public CameraWebService CameraWebService { get; set; }

        #endregion

        [Parameter]
        public int Id { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        #region FORM

        MudForm form;
        UpdateCameraIncidenteRecordingForm IncidentRecordingForm { get; set; } = new();
        private bool success;
        private string[] errors = { };


        #endregion

        private bool visible = false;

        protected override async Task OnParametersSetAsync()
        {
            var camera = await CameraWebService.GetConfigIncidentRecording(Id);

            if (camera is not null)
                IncidentRecordingForm.MacAddress = camera.MacAddress;
        }
   
        private async Task Submit()
        {

            var updateRequest = new CameraIncidentRecordingForm() { CameraId = Id, MacAddress = IncidentRecordingForm.MacAddress };

            visible = true;
            var updateResult = await CameraWebService.UpdateConfigIncidentRecording(updateRequest);
            visible = false;

            if (updateResult > 0)
            {
                Snackbar.Add("Camera atualizada com sucesso!", Severity.Success);
            }
            else
            {
                Snackbar.Add("Houve um erro ao cadastrar a camera, tente novamente mais tarde", Severity.Error);
            }
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
