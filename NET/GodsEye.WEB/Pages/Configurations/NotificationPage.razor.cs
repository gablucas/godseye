using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Components;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using System.Text.Json;


namespace GodsEye.WEB.Pages.Configurations
{
    public partial class NotificationPage
    {
        [Inject]
        public NotificationGroupWebService notificationGroupWebService { get; set; }

        #region TABLE PARAMETERS

        private List<NotificationGroupModel> _logs = new();
        private List<NotificationGroupModel> _filteredLogs = new();


        private MudTable<NotificationGroupModel> _mudTable;
        bool _loading;
        #endregion

        #region TABLE FILTERS

        private string _groupNameFilter = "";
        private string _grouEmailFilter = "";

        #endregion

        [Inject]
        public IDialogService DialogService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var result = await notificationGroupWebService.GetAllAsync();

            if (result.Success)
            {
                _logs = result.Data.ToList();
                _filteredLogs = _logs;
            }

            _loading = false;
        }

        void ApplyFilters()
        {
            _filteredLogs = _logs
                .Where(x =>
                    (string.IsNullOrWhiteSpace(_groupNameFilter) || x.Name.Contains(_groupNameFilter, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(_grouEmailFilter) || x.Emails.Any(y => y.Name.Contains(_grouEmailFilter, StringComparison.OrdinalIgnoreCase)))
                ).ToList();
        }

        private void OpenCreateNotificationGroup()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.Large };
            DialogService.ShowAsync<CreateNotificationGroupComponent>("Criar grupo email", options);
        }

        private async Task OpenUpdateNotificationGroup(NotificationGroupModel notification)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.Large };
            var parameters = new DialogParameters<UpdateNotificationGroupComponent> { { x => x.NotificationGroupModel, notification } };
            var dialog = await DialogService.ShowAsync<UpdateNotificationGroupComponent>("Editar grupo email", parameters, options);

            var result = await dialog.Result;

            if (!result.Canceled)
            {
                var updatedItem = result.Data.As<UpdateNotificationGroupForm>();

                if (updatedItem.NewEmails.Any())
                {
                    var updatedNotification = await notificationGroupWebService.GetById(notification.Id);

                    if (updatedNotification.Success)
                    {
                        var index = _logs.FindIndex(x => x.Id == notification.Id);

                        if (index != -1)
                        {
                            _logs[index] = updatedNotification.Data;
                        }
                    }
                }

                if (updatedItem.RemoveEmails.Any())
                {
                    var index = _logs.FindIndex(x => x.Id == notification.Id);
                    if (index == -1)
                        return;

                    var current = _logs[index];

                    var updatedNotification = new NotificationGroupModel
                    {
                        Id = current.Id,
                        Name = current.Name,
                        EmailsJson = JsonSerializer.Serialize(current.Emails.Where(e => !updatedItem.RemoveEmails.Contains(e.Id)).ToList())
                    };

                    _logs[index] = updatedNotification;
                    ApplyFilters();
                }
            }
        }
    }
}
