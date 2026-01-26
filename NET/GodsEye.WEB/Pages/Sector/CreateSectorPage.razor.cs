using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Pages.Sector
{
    public partial class CreateSectorPage
    {
        #region Injections

        [Inject]
        SectorService sectorService { get; set; }

        [Inject]
        CameraWebService cameraService { get; set; }

        #endregion

        #region FORM

        MudForm form;


        CreateSectorForm SectorModel { get; set; } = new();
        ApiResponse<ProcedureResult?>? apiResponse { get; set; } = null;

        bool success;
        string[] errors = { };
        bool visible = false;

        #endregion

        private void BackToRegister()
        {
            apiResponse = null;
        }

        private async Task Submit()
        {
            visible = true;
            apiResponse = await sectorService.CreateAsync(SectorModel);
            visible = false;

            if (!apiResponse.Success)
                Snackbar.Add("Houve um erro ao cadastrar o setor, tente novamente mais tarde", Severity.Error);
            else
                Snackbar.Add("Setor cadastrado com sucesso!", Severity.Success);
        }
    }
}

