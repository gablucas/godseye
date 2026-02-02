using GodsEye.WEB.Components.PersonComponents;
using MudBlazor;

namespace GodsEye.WEB.Services
{
    public class DialogWebService
    {
        private readonly IDialogService _dialogService;

        public DialogWebService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task OpenPersonInfoDialog(int personId)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False };
            var parameters = new DialogParameters<InfoPersonComponent> { { x => x.Id, personId } };

            await _dialogService.ShowAsync<InfoPersonComponent>("Informações pessoa", parameters, options);
        }
    }
}
