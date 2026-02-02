using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Components.PersonComponents;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;


namespace GodsEye.WEB.Pages.Person
{
    public partial class PersonListPage
    {
        [Inject]
        public PersonService personService { get; set; }

        [Inject]
        public SectorWebService SectorService { get; set; }

        [Inject]
        public DialogWebService DialogWebService { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        #region TABLE PARAMETERS

        private MudTable<PersonModel> mudTable;
        List<PersonModel> _persons = new();
        IEnumerable<PersonModel> _filteredPersons = Enumerable.Empty<PersonModel>();
        private int selectedRowNumber = -1;

        #endregion

        #region TABLE FILTERS
        private string _personNameFilter = "";

        private List<SectorModel> _sectors = new();
        private IEnumerable<string> _selectedSectors { get; set; } = new HashSet<string>() { };

        private string _personFilter = "";

        #endregion

        bool _loading;

        
        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var personsResult = await personService.GetAllAsync();

            if (personsResult is not null && personsResult.Success)
                _persons = personsResult.Data.ToList();
                _filteredPersons = _persons;

            _loading = false;

            var sectorsRequest = await SectorService.GetAllAsync();
            if (sectorsRequest.Success)
                _sectors = sectorsRequest.Data.ToList();
        }

        private async Task RowClickEvent(TableRowClickEventArgs<PersonModel> args)
        {
            if (args?.Item == null)
                return;

            if (args.Item.Id <= 0)
                return;

            await DialogWebService.OpenPersonInfoDialog(args.Item.Id);
        }

        private string SelectedRowClassFunc(PersonModel element, int rowNumber)
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

        private async Task OpenCreatePerson()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False };
            var dialog = await DialogService.ShowAsync<CreatePersonComponent>("Criar pessoa", options);

            var result = await dialog.Result;

            if (result.Canceled)
                return;

            if (result.Data is not int personId || personId <= 0)
            {
                Snackbar.Add("ID inválido retornado", Severity.Error);
                return;
            }

            var newPerson = await personService.GetById(personId);

            if (newPerson is null || !newPerson.Success)
                return;

            _persons.Insert(0, newPerson.Data);
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

        void ApplyFilters()
        {
            _filteredPersons = _persons
                .Where(x =>
                    (string.IsNullOrWhiteSpace(_personNameFilter) || x.Name.Contains(_personNameFilter, StringComparison.OrdinalIgnoreCase)) &&
                    (_selectedSectors.Count() == 0 || x.Sectors.Any(s => _selectedSectors.ToList().Contains(s.SectorId.ToString())))
                ).ToList();
        }
    }
}
