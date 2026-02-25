using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Components.EnvironmentMonitoringComponent;
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

        public async Task<IDialogReference?> OpenPersonInfoDialog(int personId)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False };
            var parameters = new DialogParameters<InfoPersonComponent> { { x => x.Id, personId } };

            return await _dialogService.ShowAsync<InfoPersonComponent>("Informações pessoa", parameters, options);
        }

        public async Task<IDialogReference?> OpenEnvironmentMonitoringSectorDialog(List<EnvironmentMonitoringModel> logs)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False };
            var parameters = new DialogParameters<EnvironmentMonitoringSectorComponent> { { x => x.EnvironmentMonitoringLog,  logs } };

            return await _dialogService.ShowAsync<EnvironmentMonitoringSectorComponent>("Pessoas no setor", parameters, options);
        }
    }
}
