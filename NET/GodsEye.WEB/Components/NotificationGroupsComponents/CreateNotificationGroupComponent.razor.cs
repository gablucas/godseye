using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.NotificationGroupsComponents
{
    public partial class CreateNotificationGroupComponent
    {
        [Inject]
        NotificationGroupWebService notificationGroupWebService { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public int Id { get; set; }

        #region FORM

        MudForm form;
        CreateNotificationGroupForm NotificationGroupForm { get; set; } = new();
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

            if (NotificationGroupForm.Emails.Contains(_email))
            {
                _hasEmailError = true;
                _emailErrorMessage = "E-mail já adicionado";
                return;
            }

            NotificationGroupForm.Emails.Add(_email);

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
            NotificationGroupForm.Emails.Remove(email);
        }

        #endregion

        private async Task Submit()
        {
            visible = true;
            var result = await notificationGroupWebService.CreateAsync(NotificationGroupForm);
            visible = false;

            if (result is not null)
            {
                Snackbar.Add("Camera atualizada com sucesso!", Severity.Success);
                success = false;
                MudDialog.Close(DialogResult.Ok(result.Id));
            }
            else
            {
                Snackbar.Add("Houve um erro ao cadastrar a camera, tente novamente mais tarde", Severity.Error);
            }
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
