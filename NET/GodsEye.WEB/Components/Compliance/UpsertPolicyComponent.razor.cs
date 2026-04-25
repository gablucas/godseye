using GodsEye.Domain.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.Compliance
{
    public partial class UpsertPolicyComponent
    {
        // Implementa o componente base UpsertComponentBase

        #region DI

        #endregion

        #region FORM
        protected ComplianceRuleEnum RuleType { get; set; }

        #endregion

        #region LIFETIME FUNCTIONS

        #endregion

        #region PARAMS

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        #endregion

        async Task Submit()
        {
            await form.Validate();
        }


        private void Cancel() => MudDialog.Cancel();
    }
}
