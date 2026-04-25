using GodsEye.Application.DTOs.Model;
using GodsEye.Shared.Response.EnvironmentMonitoring;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace GodsEye.WEB.Components.Dashboard
{
    public partial class EnvironmentMonitoringSectorsDashboard
    {
        [Inject]
        public EnvironmentMonitoringWebService environmentMonitoringService { get; set; }

        [Inject]
        public SignalRService SignalR { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public DialogWebService DialogWebService { get; set; }


        private List<GetEnviromentMonitoringPerSectorResponse> _logs = new();

        private HubConnection? hubConnection;
        bool _loading;


        [Inject]
        public IDialogService DialogService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var result = await environmentMonitoringService.GetSectors();

            if (result is null)
            {
                _logs = result.ToList();
            }

            _loading = false;

            SignalR.Create("https://localhost:7010/createdDataHub");

            SignalR.On<EnvironmentMonitoringLogResponse>(
                "CreatedEnvironmentMonitoring",
                log =>
                {
                    _logs = _logs.Select(x =>
                    {
                        if (x.EnvironmentMonitoringLog.Any(x => x.PersonId == log.PersonId))
                        {
                            x.TotalPerson -= 1;
                            x.EnvironmentMonitoringLog.RemoveAll(x => x.PersonId == log.PersonId);
                        }

                        if (log.SectorId == x.SectorId)
                        {
                            x.TotalPerson += 1;
                            x.EnvironmentMonitoringLog.Add(new EnvironmentMonitoringLogResponse()
                            {
                                PersonId = log.PersonId,
                                Person = log.Person,
                                IdentifiedAt = log.IdentifiedAt,
                                PersonPhoto = log.PersonPhoto
                            });
                        }

                        return x;
                    }).ToList();


                    InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });
                });

            await SignalR.StartAsync();
        }
    }
}
