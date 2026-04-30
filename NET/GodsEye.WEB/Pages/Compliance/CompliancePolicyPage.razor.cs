
using GodsEye.Shared.Response.Compliance;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace GodsEye.WEB.Pages.Compliance
{
    public partial class CompliancePolicyPage
    {
        // Configurações base herdadas da classe InfoPageBase

        #region DI

        [Inject]
        public IDialogService DialogService { get; set; }
            
        #endregion

        #region TABLE PARAMETERS

        private string _policyNameFilter = "";
        
        private MudTable<CompliancePolicyResponse> mudTable;
        private HubConnection? hubConnection;

        bool _loading;
        private int? selectedId = null;
        private int selectedRowNumber = -1;

        #endregion

        private List<BreadcrumbItem> _breadCrumb =
        [
            new("Home", href: "/"),
            new("Compliance", href: null, disabled: true),
            new("Políticas", href: null, disabled: true)
        ];



        #region TABLE FUNCTIONS
        private void RowClickEvent(TableRowClickEventArgs<CompliancePolicyResponse> tableRowClickEventArgs)
        {
            //var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.Large };
            //var parameters = new DialogParameters<ComplianceRealTimeComponent> { { x => x.Camera, tableRowClickEventArgs.Item } };

            //DialogService.ShowAsync<ComplianceRealTimeComponent>("Compliance", parameters, options);
        }

        private string SelectedRowClassFunc(CompliancePolicyResponse element, int rowNumber)
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

        protected override void ApplyFilters()
        {
            _filteredItems = _items
                .Where(x =>
                    (string.IsNullOrWhiteSpace(_policyNameFilter) || x.Name.Contains(_policyNameFilter, StringComparison.OrdinalIgnoreCase))
                ).ToList();
        }

        #endregion
    }
}
