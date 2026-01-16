using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Components;
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

        IEnumerable<PersonModel> _persons = Enumerable.Empty<PersonModel>();
        bool _loading;

        private int selectedRowNumber = -1;
        private MudTable<PersonModel> mudTable;


        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var personsResult = await personService.GetAllAsync();

            if (personsResult is not null && personsResult.Sucesso)
                _persons = personsResult.Dados;

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
    }
}
