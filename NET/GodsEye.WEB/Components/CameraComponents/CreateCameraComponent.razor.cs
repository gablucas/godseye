using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.CameraComponents
{
    public partial class CreateCameraComponent
    {
        #region DI


        #endregion

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        bool success;
        string[] errors = { };
        bool visible = false;

        protected override async Task OnInitializedAsync()
        {

        }


        private async Task Submit()
        {
            //visible = true;
            //apiResponse = await sectorService.CreateAsync(CreateSectorForm);
            //visible = false;

            //if (apiResponse.Success)
            //{
            //    Snackbar.Add("Setor cadastrado com sucesso!", Severity.Success);
            //    MudDialog.Close(DialogResult.Ok(1));
            //}
            //else
            //{
            //    Snackbar.Add("Houve um erro ao cadastrar o setor, tente novamente mais tarde", Severity.Error);
            //}

        }

        private void Cancel() => MudDialog.Cancel();
    }
}
