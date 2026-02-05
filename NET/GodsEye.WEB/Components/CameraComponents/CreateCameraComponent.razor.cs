using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.CameraComponents
{
    public partial class CreateCameraComponent
    {
        #region DI

        [Inject]
        CameraWebService cameraService { get; set; }

        [Inject]
        SectorWebService sectorService { get; set; }

        [Inject]
        FeatureWebService featureWebService { get; set; }

        #endregion

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        #region FORM

        MudForm form;
        private bool success;
        private string[] errors = { };
        private string featureError;
        public CreateCameraForm CameraModel { get; set; } = new();
        IEnumerable<SectorModel> _sectors = Enumerable.Empty<SectorModel>();
        IEnumerable<FeatureModel> _features = Enumerable.Empty<FeatureModel>();

        ApiResponse<ProcedureResult?>? apiResponse { get; set; } = null;

        private bool visible = false;

        #endregion


        [Parameter]
        public int Id { get; set; }


        protected override async Task OnInitializedAsync()
        {
            var response = await sectorService.GetAllAsync();
            if (response is not null && response.Success)
            {
                _sectors = response.Data;
            }

            var featureResponse = await featureWebService.GetAllAsync();
            if (featureResponse is not null && featureResponse.Success)
            {
                _features = featureResponse.Data;
            }
        }

        private void OnFeatureToggled(int featureId, bool isChecked)
        {

            var list = CameraModel.Features.ToList();

            if (isChecked)
            {
                if (!list.Contains(featureId))
                    list.Add(featureId);
            }
            else
            {
                list.Remove(featureId);
            }

            CameraModel.Features = list;

            ValidateFeatures();
        }

        private bool ValidateFeatures()
        {
            if (CameraModel.Features == null || !CameraModel.Features.Any())
            {
                featureError = "Selecione pelo menos uma funcionalidade";
                success = false;
            }

            featureError = null;
            return true;
        }

        private void BackToRegister()
        {
            apiResponse = null;
        }

        private async Task Submit()
        {
            if (!ValidateFeatures())
                return;

            visible = true;
            apiResponse = await cameraService.CreateAsync(CameraModel);
            visible = false;

            if (apiResponse.Success)
            {
                Snackbar.Add("camera cadastrada com Success!", Severity.Success);
                MudDialog.Close(DialogResult.Ok(apiResponse.Data.Id));
            }
            else
            {
                Snackbar.Add("Houve um erro ao cadastrar a camera, tente novamente mais tarde", Severity.Error);
            }   
        }


        private void Cancel() => MudDialog.Cancel();
    }
}
