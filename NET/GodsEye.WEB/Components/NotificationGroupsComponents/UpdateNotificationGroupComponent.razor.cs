using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.NotificationGroupsComponents
{
    public partial class UpdateNotificationGroupComponent
    {
        [Inject]
        NotificationGroupWebService notificationGroupWebService { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public int Id { get; set; }

        [Parameter]
        public NotificationGroupModel NotificationGroupModel { get; set; }

        #region FORM

        MudForm form;
        UpdateNotificationGroupForm NotificationGroupForm { get; set; } = new();
        private bool success;
        private string[] errors = { };


        private string _email;
        private bool _hasEmailError;
        private string _emailErrorMessage;

        #endregion

        private bool visible = false;

        #region Email Funcs

        protected override void OnParametersSet()
        {
            NotificationGroupForm = new UpdateNotificationGroupForm() 
            { 
                Id = NotificationGroupModel.Id,
                Name = NotificationGroupModel.Name,
                Emails = NotificationGroupModel.Emails
            };
        }

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

            if (NotificationGroupForm.Emails.Any(x => x.Name == _email))
            {
                _hasEmailError = true;
                _emailErrorMessage = "E-mail já adicionado";
                return;
            }

            NotificationGroupForm.NewEmails.Add(_email);

            _email = string.Empty;
        }

        private void OnEmailChanged(string value)
        {
            _email = value;
            _hasEmailError = false;
            _emailErrorMessage = null;
        }

        private void RemoveEmail(int id)
        {
            NotificationGroupForm.Emails.RemoveAll(x => x.Id == id);
            NotificationGroupForm.RemoveEmails.Add(id);
        }

        private void RemoveEmail(string email)
        {
            NotificationGroupForm.NewEmails.Remove(email);
        }

        #endregion

        private async Task Submit()
        {
            visible = true;
            var apiResponse = await notificationGroupWebService.UpdateAsync(NotificationGroupForm);
            visible = false;

            if (apiResponse.Success)
            {
                Snackbar.Add("Camera atualizada com sucesso!", Severity.Success);
                success = false;
                MudDialog.Close(DialogResult.Ok(NotificationGroupForm));
            }
            else
            {
                Snackbar.Add("Houve um erro ao cadastrar a camera, tente novamente mais tarde", Severity.Error);
            }
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
