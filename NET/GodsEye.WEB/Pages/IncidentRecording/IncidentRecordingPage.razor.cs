using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Components.IncidentRecordingComponents;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace GodsEye.WEB.Pages.IncidentRecording
{
    public partial class IncidentRecordingPage
    {
        [Inject]
        public IncidentRecordingWebService incidentRecordingWebService { get; set; }

        [Inject]
        public SignalRService SignalR { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        #region TABLE PARAMETERS

        private List<IncidentRecordingModel> _log = new();
        private MudTable<IncidentRecordingModel> mudTable;
        private HubConnection? hubConnection;
        bool _loading;
        private int? selectedId = null;
        private int selectedRowNumber = -1;

        #endregion

        private List<BreadcrumbItem> _items =
        [
            new("Home", href: "/"),
            new("Monitoramento", href: null, disabled: true),
            new("Incidentes", href: null, disabled: true)
        ];

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var result = await incidentRecordingWebService.GetAllLogs(1, 5);

            if (result.Success)
                _log = result.Data.ToList();

            _loading = false;

            SignalR.Create("https://localhost:7010/createdDataHub");

            SignalR.On<IncidentRecordingModel>(
                "CreatedIncidentRecording",
                log =>
                {
                    Console.WriteLine("📥 LOG RECEBIDO NO FRONT");

                    _log.Insert(0, log);

                    InvokeAsync(() =>
                    {
                        mudTable?.ReloadServerData();
                        StateHasChanged();
                    });
                });

            await SignalR.StartAsync();
        }

        


        #region TABLE FUNCTIONS
        private void RowClickEvent(TableRowClickEventArgs<IncidentRecordingModel> tableRowClickEventArgs)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.Large };
            var parameters = new DialogParameters<InfoIncidentRecordingComponent> { { x => x.IncidentRecording, tableRowClickEventArgs.Item } };

            DialogService.ShowAsync<InfoIncidentRecordingComponent>("Registro de incidentes", parameters, options);
        }

        private string SelectedRowClassFunc(IncidentRecordingModel element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                return string.Empty;
            }
            else if (mudTable.SelectedItem != null && mudTable.SelectedItem.Equals(element))
            {
                selectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion
    }
}
