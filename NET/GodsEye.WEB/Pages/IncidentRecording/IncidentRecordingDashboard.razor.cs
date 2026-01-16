using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace GodsEye.WEB.Pages.IncidentRecording
{
    public partial class IncidentRecordingDashboard
    {
        [Inject]
        public IncidentRecordingWebService incidentRecordingWebService { get; set; }

        [Inject]
        public SignalRService SignalR { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        #region TABLE PARAMETERS

        private List<IncidentRecordingModel> _log = new();
        private MudTable<IncidentRecordingModel> _mudTable;
        private HubConnection? hubConnection;
        bool _loading;

        #endregion


        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var result = await incidentRecordingWebService.GetAllLogs();

            if (result.Sucesso)
                _log = result.Dados.ToList();

            _loading = false;

            SignalR.Create("https://localhost:7010/incidentRecordingHub");

            SignalR.On<IncidentRecordingModel>(
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
