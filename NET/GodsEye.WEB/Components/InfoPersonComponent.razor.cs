using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace GodsEye.WEB.Components
{
    public partial class InfoPersonComponent
    {
        [Inject]
        public PersonService personService { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public PersonModel Person { get; set; }

        #region TABLE PARAMETERS

        private IEnumerable<PersonLogModel> _log;
        private MudTable<PersonLogModel> _mudTable;
        bool _loading;

        #endregion

        
        private void Submit() => MudDialog.Close(DialogResult.Ok(true));

        private void Cancel() => MudDialog.Cancel();

        protected override async Task OnParametersSetAsync()
        {
            var result = await personService.GetLogs(Person.Id);

            if (result.Sucesso)
                _log = result.Dados;
        }

    }
}
