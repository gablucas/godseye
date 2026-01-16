using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Pages.Camera
{
    public partial class CreateCameraPage
    {
        #region Injections

        [Inject]
        CameraService cameraService { get; set; }

        [Inject]
        SectorService sectorService { get; set; }

        #endregion

        #region FORM

        MudForm form;
        private bool success;
        private string[] errors = { };
        public CreateCameraForm CameraModel { get; set; } = new();
        IEnumerable<SectorModel> _sectors = Enumerable.Empty<SectorModel>();

        ApiResponse<ProcedureResult?>? apiResponse { get; set; } = null;

        private bool visible = false;

        #endregion

        protected override async Task OnInitializedAsync()
        {
            var response = await sectorService.GetAllAsync();
            if (response is not null && response.Sucesso)
            {
                _sectors = response.Dados;
            }
        }


        private string GetSelectedCameraNames(List<string> ids)
        {
            var names = _sectors
                .Where(c => ids.Contains(c.Id.ToString()))
                .Select(c => c.Name);

            return string.Join(", ", names);
        }


        private void BackToRegister()
        {
            apiResponse = null;
        }

        private async Task Submit()
        {
            visible = true;
            apiResponse = await cameraService.CreateAsync(CameraModel);
            visible = false;

            if (!apiResponse.Sucesso)
                Snackbar.Add("Houve um erro ao cadastrar a camera, tente novamente mais tarde", Severity.Error);
            else
                Snackbar.Add("camera cadastrada com sucesso!", Severity.Success);
        }
    }
}
