using GodsEye.Shared.Enums;
using GodsEye.Shared.Response.Compliance;
using GodsEye.Shared.Response.Sector;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;

namespace GodsEye.WEB.Pages.Compliance
{
    public partial class SectorTransitionViolationTable
    {
        // Configurações base herdadas da classe InfoPageBase

        #region DI

        [Inject]
        public IDialogService DialogService { get; set; }

        [Inject]
        public DialogWebService DialogWebService { get; set; }

        [Inject]
        public SectorWebService SectorService { get; set; }

        [Inject]
        public ComplianceWebService ComplianceWebService { get; set; }

        [Inject]
        public SignalRService SignalR { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        #endregion

        #region TABLE

        private MudTable<ComplianceViolationResponse> mudTable;
        private HubConnection? hubConnection;

        bool _loading;
        private int selectedRowNumber = -1;

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

        #endregion

        #region FILTERS

        private bool showFilters = false;
        private int _filterCounter = 0;

        private string _personNameFilter = "";

        private List<SectorResponse> _sectors = new();
        private HashSet<string> _selectedSectors { get; set; } = new HashSet<string>();

        private HashSet<CompliancePolicyEnum> _selectedPoliciesType = new HashSet<CompliancePolicyEnum>();
        private HashSet<ComplianceViolationEnum> _selectedViolationType = new HashSet<ComplianceViolationEnum>();

        private void FilterCounter()
        {
            _filterCounter = 0;

            if (!string.IsNullOrEmpty(_personNameFilter))
                _filterCounter += 1;

            _filterCounter += _selectedSectors.Count;
            _filterCounter += _selectedPoliciesType.Count;
            _filterCounter += _selectedViolationType.Count;
        }

        protected override void ApplyFilters()
        {
            _filteredItems = _items
                .Where(x => (string.IsNullOrWhiteSpace(_personNameFilter) || x.PersonName.Contains(_personNameFilter, StringComparison.OrdinalIgnoreCase)) &&
                (_selectedSectors.Count == 0 || _selectedSectors.Contains(x.SectorId.ToString())) &&
                (_selectedPoliciesType.Count == 0 || _selectedPoliciesType.Contains(x.PolicyType)) &&
                (_selectedViolationType.Count == 0 || _selectedViolationType.Contains(x.ViolationType))
                ).ToList();

            FilterCounter();
        }

        private void CleanFilters()
        {
            _personNameFilter = "";
            _selectedSectors.Clear();
            _selectedPoliciesType.Clear();
            _selectedViolationType.Clear();

            ApplyFilters();
            FilterCounter();
        }

        private List<string> _PDFColumns = new();

        private void HandlerPDFColumns(bool isChecked, string column)
        {
            if (isChecked)
                _PDFColumns.Add(column);
            else
                _PDFColumns.Remove(column);
        }


        #endregion

        #region LIFEFICLE FUNCTIONS

        // TEMPORARIO
        public IEnumerable<SectorTransitionResponse> transitionRules { get; set; }

        protected override async Task OnBeforeLoad()
        {
            var sectorTransitionRules = await ComplianceWebService.GetAllSectorTransition();

            if (sectorTransitionRules is not null)
                transitionRules = sectorTransitionRules.ToList();

            var sectorsRequest = await SectorService.GetAllAsync();
            if (sectorsRequest is not null)
                _sectors = sectorsRequest.ToList();

            await LoadSignalR();
        }

        #endregion

        #region COMPLIANCE MENU

        private bool _dense = false;
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

        #region SIGNALR

        private async Task LoadSignalR()
        {
            SignalR.Create($"{Configuration["ApiUrl"]}/createdDataHub");

            // Adicionado 'async' antes de 'logId'
            SignalR.On<int>("CreatedComplianceViolation", async logId =>
            {

                try
                {
                    var complianceViolation = await Service.GetById(logId);

                    if (complianceViolation is null) return;

                    await InvokeAsync(() =>
                    {
                        _items.Insert(0, complianceViolation);
                        mudTable?.ReloadServerData();

                        StateHasChanged();
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar SignalR: {ex.Message}");
                }
            });

            await SignalR.StartAsync();
        }

        #endregion

        private async Task DownloadPDF()
        {
            var url = await Service.GetTransitionViolationPDF();

            await JS.InvokeVoidAsync("downloadFile", url, "relatorio.pdf");
        }
    }
}
