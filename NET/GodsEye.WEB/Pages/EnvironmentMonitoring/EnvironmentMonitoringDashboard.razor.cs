using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace GodsEye.WEB.Pages.EnvironmentMonitoring
{
    public partial class EnvironmentMonitoringDashboard
    {
        [Inject]
        public EnvironmentMonitoringService environmentMonitoringService { get; set; }

        [Inject]
        public SignalRService SignalR { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        #region TABLE PARAMETERS

        private List<EnvironmentMonitoringModel> _log = new();
        private MudTable<EnvironmentMonitoringModel> _mudTable;
        private HubConnection? hubConnection;
        bool _loading;

        #endregion


        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var result = await environmentMonitoringService.GetAllLogs();

            if (result.Sucesso)
                _log = result.Dados.ToList();

            _loading = false;

            SignalR.Create("https://localhost:7010/environmentMonitoringHub");

            SignalR.On<EnvironmentMonitoringModel>(
                "ReceiveMessage",
                log =>
                {
                    Console.WriteLine("📥 LOG RECEBIDO NO FRONT");

                    _log.Insert(0, log);

                    InvokeAsync(() =>
                    {
                        _mudTable?.ReloadServerData();
                        StateHasChanged();
                    });
                });

            await SignalR.StartAsync();
        }
    }
}
