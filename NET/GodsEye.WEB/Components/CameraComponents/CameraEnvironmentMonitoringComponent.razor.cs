using GodsEye.WEB.Model.Forms;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.CameraComponents
{
    public partial class CameraEnvironmentMonitoringComponent
    {
        #region DI


        #endregion

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        #region FORM

        MudForm form;
        UpdateCameraDwellTimeMonitoringForm DwellTimeMonitoringForm { get; set; } = new();
        private bool success;
        private string[] errors = { };
        private bool visible = false;

        #endregion

        #region PARAMS

        [Parameter]
        public int Id { get; set; }

        #endregion

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
