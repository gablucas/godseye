
using GodsEye.Shared.Enums;
using GodsEye.Shared.Response.Compliance;
using GodsEye.Shared.Response.Sector;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace GodsEye.WEB.Pages.Compliance
{
    public partial class ComplianceViolationsPage
    {
        // Configurações base herdadas da classe InfoPageBase

        #region DI

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public SectorWebService SectorService { get; set; }

        #endregion

        #region TABLE PARAMETERS

        private MudTable<ComplianceViolationResponse> mudTable;
        private HubConnection? hubConnection;

        bool _loading;
        private int selectedRowNumber = -1;

        #endregion

        #region TABLE FILTERS

        private string _personNameFilter = "";


        private List<SectorResponse> _sectors = new();
        private HashSet<string> _selectedSectors { get; set; } = new HashSet<string>();

        private HashSet<CompliancePolicyEnum> _selectedPoliciesType = new HashSet<CompliancePolicyEnum>();
        private HashSet<ComplianceViolationEnum> _selectedViolationType = new HashSet<ComplianceViolationEnum>();

        #endregion

        #region LIFEFICLE FUNCTIONS

        protected override async Task OnBeforeLoad()
        {
            var sectorsRequest = await SectorService.GetAllAsync();
            if (sectorsRequest is not null)
                _sectors = sectorsRequest.ToList();
        }

        #endregion

        #region COMPLIANCE MENU

        private CompliancePolicyEnum? _selectedMenu = null;

        private void SelectMenuOption(CompliancePolicyEnum? selectedMenu)
        {
            _selectedMenu = selectedMenu;
        }

        private string IsMenuSelected(CompliancePolicyEnum? selectedMenu)
        {
            if (_selectedMenu == selectedMenu)
                return "selected";

            return "";
        }

        #endregion

        private List<BreadcrumbItem> _breadCrumb =
        [
            new("Home", href: "/"),
            new("Compliance", href: null, disabled: true),
            new("Violações", href: null, disabled: true)
        ];


        #region TABLE FUNCTIONS

        private void RowClickEvent(TableRowClickEventArgs<ComplianceViolationResponse> tableRowClickEventArgs)
        {
            //selectedId = tableRowClickEventArgs.Item.Id;
        }

        private string SelectedRowClassFunc(ComplianceViolationResponse element, int rowNumber)
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
                .Where(x => (string.IsNullOrWhiteSpace(_personNameFilter) || x.PersonName.Contains(_personNameFilter, StringComparison.OrdinalIgnoreCase)) &&
                (_selectedSectors.Count == 0 || _selectedSectors.Contains(x.SectorId.ToString())) &&
                (_selectedPoliciesType.Count == 0 || _selectedPoliciesType.Contains(x.PolicyType)) &&
                (_selectedViolationType.Count == 0 || _selectedViolationType.Contains(x.ViolationType))
                ).ToList();
        }

        #endregion
    }
}
