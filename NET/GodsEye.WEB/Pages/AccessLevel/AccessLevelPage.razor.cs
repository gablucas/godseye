using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Components.AccessLevelComponents;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Pages.AccessLevel
{
    public partial class AccessLevelPage
    {
        #region DI

        [Inject]
        public AccessLevelWebService AccessLevelWebService { get; set; }

        [Inject]
        public DialogWebService DialogWebService { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        #endregion

        #region TABLE PARAMETERS

        private MudTable<AccessLevelModel> mudTable;
        List<AccessLevelModel> _accessLevel = new();
        IEnumerable<AccessLevelModel> _filteredAccessLevel = Enumerable.Empty<AccessLevelModel>();
        private int selectedRowNumber = -1;

        #endregion

        #region TABLE FILTERS
        private string _personNameFilter = "";

        private List<SectorModel> _sectors = new();
        private IEnumerable<string> _selectedSectors { get; set; } = new HashSet<string>() { };

        private string _personFilter = "";

        #endregion

        private List<BreadcrumbItem> _items =
        [
            new("Home", href: "/"),
            new("Cadastro", href: null, disabled: true),
            new("Pessoas", href: null, disabled: true)
        ];

        bool _loading;


        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var accessLevelRequest = await AccessLevelWebService.GetAllAsync();

            if (accessLevelRequest.Success)
            {
                _accessLevel = accessLevelRequest.Data.ToList();
                _filteredAccessLevel = _accessLevel.ToList();
            }
                
            _loading = false;
        }

        private async Task RowClickEvent(TableRowClickEventArgs<AccessLevelModel> args)
        {
            if (args?.Item == null)
                return;

            if (args.Item.Id <= 0)
                return;

            await DialogWebService.OpenPersonUpdateDialog(args.Item.Id, null);
        }

        private string SelectedRowClassFunc(AccessLevelModel element, int rowNumber)
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

        private async Task CreateAccessLevelCallback(int personId)
        {
            
            var newAccessLevel = await AccessLevelWebService.GetById(personId);

            if (newAccessLevel is null || !newAccessLevel.Success)
                return;

            _accessLevel.Insert(0, newAccessLevel.Data);
        }

        private void OnSectorsChanged(IEnumerable<string> values)
        {
            _selectedSectors = values.ToHashSet();
            ApplyFilters();
        }

        private string GetMultiSelectionText(List<string> selectedValues)
        {
            return $"{selectedValues.Count} setor{(selectedValues.Count > 1 ? "es foram selecionados" : " foi selecionado")}";
        }

        private async Task OpenEditData(int accessLevelId)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False, NoHeader = true };
            var parameters = new DialogParameters<CreateAccessLevelComponent> { { x => x.Id, accessLevelId } };
            var dialog = await DialogService.ShowAsync<CreateAccessLevelComponent>("Atualizar calendário de acesso", parameters, options);

            var result = await dialog.Result;
        }

        void ApplyFilters()
        {
            _filteredAccessLevel = _accessLevel
                .Where(x =>
                    (string.IsNullOrWhiteSpace(_personNameFilter) || x.Name.Contains(_personNameFilter, StringComparison.OrdinalIgnoreCase))
                    //(_selectedSectors.Count() == 0 || x.Sectors.Any(s => _selectedSectors.ToList().Contains(s.SectorId.ToString())))
                ).ToList();
        }
    }
}
