
using GodsEye.Shared.Response.Compliance;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace GodsEye.WEB.Components.Dashboard
{
    public partial class ComplianceViolationDashboard
    {
        [Inject]
        public ComplianceViolationWebService complianceViolationWebService { get; set; }

        [Inject]
        public SignalRService SignalR { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public DialogWebService DialogWebService { get; set; }


        private List<ComplianceViolationResponse> _logs = new();
        private List<ComplianceViolationResponse> _filteredLogs = new();

        private HubConnection? hubConnection;
        bool _loading;


        [Inject]
        public IDialogService DialogService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var result = await complianceViolationWebService.GetAllAsync(1, 5);

            if (result is not null)
            {
                _logs = result.ToList();
                _filteredLogs = _logs;
            }

            _loading = false;

            SignalR.Create("https://localhost:7010/createdDataHub");

            SignalR.On<int>(
                "CreatedComplianceViolationRecording",
                async complianceViolationId =>
                {
                    var complianceViolation = await complianceViolationWebService.GetById(complianceViolationId);

                    if (complianceViolation == null)
                        return;

                    _logs.Insert(0, complianceViolation);

                    if (_logs.Count > 5)
                    {
                        _logs.RemoveAt(_logs.Count - 1);
                    }

                    await InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });
                });

            await SignalR.StartAsync();
        }
    }
}
