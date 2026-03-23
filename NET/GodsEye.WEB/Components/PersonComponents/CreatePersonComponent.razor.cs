using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.PersonComponents
{
    public partial class CreatePersonComponent
    {
        #region DI

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        #endregion

        private void Cancel() => MudDialog.Cancel();
    }
}
