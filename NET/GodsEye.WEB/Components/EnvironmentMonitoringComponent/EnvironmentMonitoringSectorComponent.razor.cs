using GodsEye.Shared.Response.EnvironmentMonitoring;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.EnvironmentMonitoringComponent
{
    public partial class EnvironmentMonitoringSectorComponent
    {
        [Inject]
        public PersonService personService { get; set; }

        [Inject]
        public DialogWebService DialogWebService { get; set; }

        #region TABLE PARAMETERS

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public List<EnvironmentMonitoringLogResponse> EnvironmentMonitoringLog { get; set; } = new();

        private MudTable<EnvironmentMonitoringLogResponse> mudTable;

        IEnumerable<EnvironmentMonitoringLogResponse> _filteredEnvironmentMonitoringLog = Enumerable.Empty<EnvironmentMonitoringLogResponse>();
        private int selectedRowNumber = -1;

        #endregion

        #region TABLE FILTERS
        private string _personNameFilter = "";
        private string _personFilter = "";

        #endregion

        bool _loading;

        protected override void OnParametersSet()
        {
            if (EnvironmentMonitoringLog.Count > 0)
                _filteredEnvironmentMonitoringLog = EnvironmentMonitoringLog.ToList();
        }

        private async Task RowClickEvent(TableRowClickEventArgs<EnvironmentMonitoringLogResponse> args)
        {
            if (args?.Item == null)
                return;

            if (args.Item.PersonId <= 0)
                return;

            MudDialog.Close();

            var dialog = await DialogWebService.OpenPersonUpdateDialog(args.Item.PersonId, null);
        }

        private string SelectedRowClassFunc(EnvironmentMonitoringLogResponse element, int rowNumber)
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

        void ApplyFilters()
        {
            _filteredEnvironmentMonitoringLog = EnvironmentMonitoringLog
                .Where(x =>
                    (string.IsNullOrWhiteSpace(_personNameFilter) || x.Person.Contains(_personNameFilter, StringComparison.OrdinalIgnoreCase))
                ).ToList();
        }
    }
}
