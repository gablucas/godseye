using GodsEye.Application.DTOs.Model;
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


        private List<EnvironmentMonitoringSectorModel> _logs = new();

        private HubConnection? hubConnection;
        bool _loading;


        [Inject]
        public IDialogService DialogService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var result = await environmentMonitoringService.GetSectors();

            if (result.Success)
            {
                _logs = result.Data.ToList();
                
                foreach(var log in _logs)
                {
                    log.ParsePersons();
                }
            }

            _loading = false;

            SignalR.Create("https://localhost:7010/environmentMonitoringHub");

            SignalR.On<EnvironmentMonitoringModel>(
                "ReceiveMessage",
                log =>
                {
                    Console.WriteLine("📥 LOG RECEBIDO NO FRONT");
                    _logs = _logs.Select(x =>
                    {
                        if (x.PersonLog.Any(x => x.PersonId == log.PersonId))
                        {
                            x.TotalPerson -= 1;
                            x.PersonLog.RemoveAll(x => x.PersonId == log.PersonId);
                        }

                        if (log.SectorId == x.SectorId)
                        {
                            x.TotalPerson += 1;
                            x.PersonLog.Add(new EnvironmentMonitoringSectorLog()
                            {
                                PersonId = log.PersonId,
                                PersonName = log.Person,
                                CreatedAt = log.CreatedAt,
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
