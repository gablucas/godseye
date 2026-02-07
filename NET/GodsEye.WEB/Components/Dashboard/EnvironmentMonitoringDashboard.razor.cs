using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace GodsEye.WEB.Components.Dashboard
{
    public partial class EnvironmentMonitoringDashboard
    {
        [Inject]
        public EnvironmentMonitoringWebService environmentMonitoringService { get; set; }

        [Inject]
        public SectorWebService sectorWebService { get; set; }

        [Inject]
        public SignalRService SignalR { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public DialogWebService DialogWebService { get; set; }


        private List<EnvironmentMonitoringModel> _logs = new();
        private List<EnvironmentMonitoringModel> _filteredLogs = new();

        private List<SectorModel> _sector = new();
        private int _selectedSector = 0;

        private HubConnection? hubConnection;
        bool _loading;


        [Inject]
        public IDialogService DialogService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var result = await environmentMonitoringService.GetAllLogs(1, 5);

            if (result.Success)
            {
                _logs = result.Data.ToList();
                _filteredLogs = _logs;
            }

            var sectorResult = await sectorWebService.GetAllAsync();

            if (sectorResult.Success)
            {
                _sector = sectorResult.Data.ToList();
            }

            _loading = false;

            SignalR.Create("https://localhost:7010/environmentMonitoringHub");

            SignalR.On<EnvironmentMonitoringModel>(
                "ReceiveMessage",
                log =>
                {
                    Console.WriteLine("📥 LOG RECEBIDO NO FRONT");

                    _logs.Insert(0, log);

                    if (_logs.Count > 5)
                    {
                        _logs.RemoveAt(_logs.Count - 1);
                    }

                    SelectSector(_selectedSector);

                    InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });
                });

            await SignalR.StartAsync();
        }

        private void SelectSector(int sectorId)
        {
            _selectedSector = sectorId;

            if (_selectedSector == 0)
            {
                _filteredLogs = _logs;
                return;
            }
                
            _filteredLogs = _logs.Where(x => x.SectorId == _selectedSector).ToList();
        }
    }
}
