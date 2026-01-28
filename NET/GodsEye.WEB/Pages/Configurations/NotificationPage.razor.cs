using GodsEye.WEB.Model.Forms;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Pages.Configurations
{
    public partial class NotificationPage
    {
        [Parameter]
        public int Id { get; set; }

        #region FORM

        MudForm form;
        UpdateCameraIncidenteRecordingForm IncidentRecordingForm { get; set; } = new();
        private bool success;
        private string[] errors = { };


        private string _email;
        private bool _hasEmailError;
        private string _emailErrorMessage;

        #endregion

        private bool visible = false;

        #region Email Funcs

        private void AddEmail()
        {
            _hasEmailError = false;
            _emailErrorMessage = null;

            if (string.IsNullOrWhiteSpace(_email))
            {
                _hasEmailError = true;
                _emailErrorMessage = "Informe um e-mail";
                return;
            }

            _email = _email.Trim();

            if (!System.Net.Mail.MailAddress.TryCreate(_email, out _))
            {
                _hasEmailError = true;
                _emailErrorMessage = "E-mail inválido";
                return;
            }

            if (IncidentRecordingForm.Emails.Contains(_email))
            {
                _hasEmailError = true;
                _emailErrorMessage = "E-mail já adicionado";
                return;
            }

            IncidentRecordingForm.Emails.Add(_email);

            _email = string.Empty;
        }

        private void OnEmailChanged(string value)
        {
            _email = value;
            _hasEmailError = false;
            _emailErrorMessage = null;
        }

        private void RemoveEmail(string email)
        {
            IncidentRecordingForm.Emails.Remove(email);
        }

        #endregion

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
