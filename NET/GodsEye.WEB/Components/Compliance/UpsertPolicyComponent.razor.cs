using GodsEye.Shared.Enums;
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
        protected CompliancePolicyEnum RuleType { get; set; }

        #endregion

        #region LIFETIME FUNCTIONS
        protected override async Task OnParametersSetAsync()
        {
            if (Id > 0)
            {
                var policy = await Service.GetById(Id);

                if (policy != null)
                    RuleType = policy.Rule;
            }
        }

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
