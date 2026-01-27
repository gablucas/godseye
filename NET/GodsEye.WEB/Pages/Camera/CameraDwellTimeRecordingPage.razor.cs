using GodsEye.WEB.Model.Forms;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Pages.Camera
{
    public partial class CameraDwellTimeRecordingPage
    {
        [Parameter]
        public int Id { get; set; }

        #region FORM

        MudForm form;
        UpdateCameraDwellTimeMonitoringForm DwellTimeMonitoringForm { get; set; } = new();
        private bool success;
        private string[] errors = { };

        #endregion


        private bool visible = false;

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
    }
}
