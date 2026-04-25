
using GodsEye.WEB.Components;
using GodsEye.WEB.Components.Compliance;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;

namespace GodsEye.WEB.Services
{
    public class NewDialogWebService
    {
        private readonly IDialogService _dialogService;
        private readonly DialogOptions _options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False, NoHeader = true };

        public NewDialogWebService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task OpenCreateAsync<TComponent>(Func<int, Task>? callback = null) where TComponent : ComponentBase
        {
            var dialog = await _dialogService.ShowAsync<TComponent>("", _options);

            var result = await dialog.Result;

            if (result.Canceled)
                return;

            if (callback is not null)
            {
                var id = result.Data.As<int>();
                await callback(id);
            }
        }

        public async Task OpenHandleAsync<TComponent>(int? id = null, Func<int, Task>? callback = null) where TComponent : UpsertComponentBase
        {
            var parameters = new DialogParameters<TComponent>();

            if (id.HasValue)
            {
                parameters.Add(x => x.Id, id.Value);
            }

            var dialog = await _dialogService.ShowAsync<UpsertPolicyComponent>("", parameters, _options);

            var result = await dialog.Result;

            if (result.Canceled)
                return;

            if (callback is not null)
            {
                var sectorId = result.Data.As<int>();
                await callback(sectorId);
            }
        }
    }
}
