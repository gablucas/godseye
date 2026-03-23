using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.PersonComponents
{
    public partial class PersonBarComponent
    {
        [Inject]
        public DialogWebService dialogWebService { get; set; }

        [Parameter]
        public string Pagina { get; set; }

        [Parameter]
        public int PersonId { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }
    }
}
