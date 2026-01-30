using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Components;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

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
            DialogService.ShowAsync<NotificationGroupComponent>("Criar grupo email", options);
        }
    }
}
