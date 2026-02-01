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
        public IDialogService DialogService { get; set; }

        #region TABLE PARAMETERS

        private MudTable<PersonModel> mudTable;
        IEnumerable<PersonModel> _persons = Enumerable.Empty<PersonModel>();
        IEnumerable<PersonModel> _filteredPersons = Enumerable.Empty<PersonModel>();
        private int selectedRowNumber = -1;

        #endregion

        #region TABLE FILTERS

        private string _personNameFilter = "";
        private string _sectorNameFilter = "";

        #endregion

        bool _loading;

        
        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var personsResult = await personService.GetAllAsync();

            if (personsResult is not null && personsResult.Success)
                _persons = personsResult.Data;
                _filteredPersons = _persons;

            _loading = false;
        }

        private void RowClickEvent(TableRowClickEventArgs<PersonModel> tableRowClickEventArgs)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.Large };
            var parameters = new DialogParameters<InfoPersonComponent> { { x => x.Person, tableRowClickEventArgs.Item } };

            DialogService.ShowAsync<InfoPersonComponent>("Simple Dialog", parameters, options);
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

        private void OpenCreatePerson()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.Large };
            DialogService.ShowAsync<CreatePersonComponent>("Criar pessoa", options);
        }

        void ApplyFilters()
        {
            _filteredPersons = _persons
                .Where(x =>
                    (string.IsNullOrWhiteSpace(_personNameFilter) || x.Name.Contains(_personNameFilter, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(_sectorNameFilter) || x.Sectors.Any(y => y.SectorName.Contains(_sectorNameFilter, StringComparison.OrdinalIgnoreCase)))
                ).ToList();
        }
    }
}
