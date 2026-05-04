
using GodsEye.Shared.Response.EnvironmentMonitoring;
using GodsEye.Shared.Response.Sector;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace GodsEye.WEB.Pages.EnvironmentMonitoring
{
    public partial class EnvironmentMonitoringPage
    {
        [Inject]
        public EnvironmentMonitoringWebService environmentMonitoringService { get; set; }

        [Inject]
        public SignalRService SignalR { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        public SectorWebService SectorService { get; set; }

        [Inject]
        public DialogWebService DialogWebService { get; set; }

        [Inject]
        public IConfiguration Configuration { get; set; }

        #region TABLE PARAMETERS

        private List<EnvironmentMonitoringLogResponse> _logs = new();
        private List<EnvironmentMonitoringLogResponse> _filteredLogs = new();


        private MudTable<EnvironmentMonitoringLogResponse> _mudTable;
        private HubConnection? hubConnection;
        bool _loading;
        #endregion

        #region TABLE FILTERS

        private List<SectorResponse> _sectors = new();
        private string value { get; set; } = "Nothing selected";
        private IEnumerable<string> _selectedSectors { get; set; } = new HashSet<string>() { };

        private string _personFilter = "";

        private string _selectedFilterData = "";

        private List<string> _preFiltersData = new() { "Hoje", "Ontem", "Esta semana", "Semana passada", "Este mes", "Mes passado", "Este ano", "Ano passado" };

        private MudDatePicker _initialDatePicker;
        private MudDatePicker _finalDatePicker;

        private bool _changedInitialDateByPreFilter = false;
        private bool _changedFinalDateByPreFilter = false;

        DateTime? _initialDate = null;
        DateTime? _finalDate = null;

        #endregion

        [Inject]
        public IDialogService DialogService { get; set; }

        private List<BreadcrumbItem> _items =
        [
            new("Home", href: "/"),
            new("Monitoramento", href: null, disabled: true),
            new("Ambientes", href: null, disabled: true)
        ];

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var result = await environmentMonitoringService.GetAllLogs(1, 100);

            if (result is not null)
            {
                _logs = result.ToList();
                _filteredLogs = _logs;
            }
                
            _loading = false;

            SignalR.Create($"{Configuration["ApiUrl"]}/createdDataHub");

            SignalR.On<EnvironmentMonitoringLogResponse>(
                "CreatedEnvironmentMonitoring",
                log =>
                {
                    Console.WriteLine("📥 LOG RECEBIDO NO FRONT");

                    _logs.Insert(0, log);

                    InvokeAsync(() =>
                    {
                        _mudTable?.ReloadServerData();
                        StateHasChanged();
                    });
                });

            await SignalR.StartAsync();

            var sectorsRequest = await SectorService.GetAllAsync();
            if (sectorsRequest is not null)
                _sectors = sectorsRequest.ToList();
        }

        #region FILTER FUNCS

        private string GetMultiSelectionText(List<string> selectedValues)
        {
            return $"{selectedValues.Count} setor{(selectedValues.Count > 1 ? "es foram selecionados" : " foi selecionado")}";
        }

        private void OnSectorsChanged(IEnumerable<string> values)
        {
            _selectedSectors = values.ToHashSet();
            ApplyFilters();
        }

        private void PreDateFilter()
        {
            _changedInitialDateByPreFilter = true;
            _changedFinalDateByPreFilter = true;

            DateTime hoje = DateTime.Today;
            DateTime inicio;
            DateTime fim;

            switch (_selectedFilterData)
            {
                case "Hoje":
                    inicio = hoje;
                    fim = hoje;
                    break;

                case "Ontem":
                    inicio = hoje.AddDays(-1);
                    fim = inicio;
                    break;

                case "Esta semana":
                    inicio = hoje.AddDays(-(int)hoje.DayOfWeek + (int)DayOfWeek.Monday);
                    fim = inicio.AddDays(6);
                    break;

                case "Semana passada":
                    inicio = hoje.AddDays(-(int)hoje.DayOfWeek + (int)DayOfWeek.Monday).AddDays(-7);
                    fim = inicio.AddDays(6);
                    break;

                case "Este mes":
                    inicio = new DateTime(hoje.Year, hoje.Month, 1);
                    fim = inicio.AddMonths(1).AddDays(-1);
                    break;

                case "Mes passado":
                    inicio = new DateTime(hoje.Year, hoje.Month, 1).AddMonths(-1);
                    fim = inicio.AddMonths(1).AddDays(-1);
                    break;

                case "Este ano":
                    inicio = new DateTime(hoje.Year, 1, 1);
                    fim = new DateTime(hoje.Year, 12, 31);
                    break;

                case "Ano passado":
                    inicio = new DateTime(hoje.Year - 1, 1, 1);
                    fim = new DateTime(hoje.Year - 1, 12, 31);
                    break;

                default:
                    return;
            }

            _initialDatePicker.GoToDate(inicio);
            _finalDatePicker.GoToDate(fim);
            _initialDate = inicio;
            _finalDate = fim;
            ApplyFilters();
        }

        private void OnInitialDateChanged(DateTime? date)
        {
            if(!_changedInitialDateByPreFilter)
            {
               
                if (date > _finalDate)
                {
                    _initialDatePicker.GoToDate((DateTime)_finalDate);
                }
                else
                {
                    _initialDate = date;
                    ApplyFilters();
                }

                _selectedFilterData = "";
            }

            _changedInitialDateByPreFilter = false;
        }

        private void OnFinalDateChanged(DateTime? date)
        {
            if (!_changedFinalDateByPreFilter)
            {
                
                if (date < _initialDate)
                {
                    _finalDatePicker.GoToDate((DateTime)_initialDate);
                }
                else
                {
                    _finalDate = date;
                    ApplyFilters();
                }

                _selectedFilterData = "";
            }

            _changedFinalDateByPreFilter = false;
        }

        void ApplyFilters()
        {
            var initial = _initialDate?.Date;
            var final = _finalDate?.Date.AddDays(1).AddTicks(-1);

            _filteredLogs = _logs
                .Where(x =>
                    (_selectedSectors.Count() == 0 || _selectedSectors.Contains(x.SectorId.ToString())) &&
                    (string.IsNullOrWhiteSpace(_personFilter) ||
                     x.Person.Contains(_personFilter, StringComparison.OrdinalIgnoreCase)) &&
                    (initial == null || x.IdentifiedAt >= initial) &&
                    (final == null || x.IdentifiedAt <= final)
                ).ToList();
        }

        private void CleanPreFiltred()
        {
            _selectedFilterData = "";
            _initialDate = null;
            _finalDate = null;

            _initialDatePicker.ClearAsync();
            _finalDatePicker.ClearAsync();

            ApplyFilters();
        }

        #endregion

        #region DEV TOOLS

        private async Task DeleteAll()
        {
            await environmentMonitoringService.DeleteAllLogs();
            _logs.Clear();
            _filteredLogs.Clear();
        }

        #endregion
    }
}
