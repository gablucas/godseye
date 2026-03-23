using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Components.PersonComponents;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using MudBlazor;


namespace GodsEye.WEB.Pages.Person
{
    public partial class PersonPage
    {
        [Inject]
        public PersonService personService { get; set; }

        [Inject]
        public SectorWebService SectorService { get; set; }

        [Inject]
        public DialogWebService DialogWebService { get; set; }

        [Inject]
        public SignalRService SignalR { get; set; }

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

        private bool _toggleShowPersonWithNoImage = false;

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

            var personsResult = await personService.GetAllAsync();

            if (personsResult is not null && personsResult.Success)
                _persons = personsResult.Data.ToList();
                _filteredPersons = _persons;

            

            var sectorsRequest = await SectorService.GetAllAsync();
            if (sectorsRequest.Success)
                _sectors = sectorsRequest.Data.ToList();


            SignalR.Create("https://localhost:7010/createdDataHub");

            SignalR.On<PersonModel>(
                "CreatedPerson",
                person =>
                {
                    _persons = _persons.Where(x => x.Id != person.Id).ToList();
                    _persons.Insert(0, person);
                    ApplyFilters();

                    InvokeAsync(() =>
                    {
                        mudTable?.ReloadServerData();
                        StateHasChanged();
                    });
                });

            await SignalR.StartAsync();

            _loading = false;
        }

        private async Task RowClickEvent(TableRowClickEventArgs<PersonModel> args)
        {
            if (args?.Item == null)
                return;

            if (args.Item.Id <= 0)
                return;

            await DialogWebService.OpenPersonUpdateDialog(args.Item.Id, null);
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

        private async Task CreatePersonCallback(int personId)
        {
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

        private void FilterPersonsWithNoImage()
        {
            if (!_toggleShowPersonWithNoImage)
            {
                _filteredPersons = _persons.Where(x => x.ImagePath == null).ToList();
                _toggleShowPersonWithNoImage = true;
            } 
            else
            {
                ApplyFilters();
                _toggleShowPersonWithNoImage = false;
            }
        }

        void ApplyFilters()
        {
            _filteredPersons = _persons
                .Where(x =>
                    (string.IsNullOrWhiteSpace(_personNameFilter) || x.Name.Contains(_personNameFilter, StringComparison.OrdinalIgnoreCase))
                    //(_selectedSectors.Count() == 0 || x.Sectors.Any(s => _selectedSectors.ToList().Contains(s.SectorId.ToString())))
                ).ToList();
        }
    }
}
