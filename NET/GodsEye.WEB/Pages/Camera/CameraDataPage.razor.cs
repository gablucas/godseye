using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Pages.Camera
{
    public partial class CameraDataPage
    {
        #region DI

        [Inject]
        CameraWebService _cameraService { get; set; }

        [Inject]
        SectorWebService _sectorService { get; set; }

        [Inject]
        FeatureWebService _featureWebService { get; set; }

        #endregion

        #region PARAMS

        [Parameter]
        public int Id { get; set; }

        #endregion


        #region FORM

        MudForm form;
        UpdateCameraForm CameraForm { get; set; } = new();
        IEnumerable<SectorModel> _sectors = Enumerable.Empty<SectorModel>();
        IEnumerable<FeatureModel> _features = Enumerable.Empty<FeatureModel>();

        private bool success;
        private string[] errors = { };
        private string featureError;

        #endregion

        public CameraModel camera { get; set; }

        ApiResponse<ProcedureResult?>? apiResponse { get; set; } = null;

        private bool visible = false;

        Guid _refreshToken = Guid.NewGuid();

        protected override async Task OnParametersSetAsync()
        {

            var result = await _cameraService.GetById(Id);

            if (result.Success && result is not null && result.Data is not null)
            {
                camera = result.Data;

                CameraForm = new UpdateCameraForm()
                {
                    Id = camera.Id,
                    Name = camera.Name,
                    Connection = camera.Connection,
                    Features = camera.Features.Select(x => x.FeatureId),
                    SectorId = camera.SectorId.ToString()
                };
            }

            base.OnParametersSet();
        }

        protected override void OnInitialized()
        {
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;

            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            var response = await _sectorService.GetAllAsync();
            if (response is not null && response.Success)
            {
                _sectors = response.Data;
            }

            var featureResponse = await _featureWebService.GetAllAsync();
            if (featureResponse is not null && featureResponse.Success)
            {
                _features = featureResponse.Data;
            }
        }


        #region Feature Validation
        private void OnFeatureToggled(int featureId, bool isChecked)
        {

            var list = CameraForm.Features.ToList();

            if (isChecked)
            {
                if (!list.Contains(featureId))
                    list.Add(featureId);
            }
            else
            {
                list.Remove(featureId);
            }

            CameraForm.Features = list;

            //ValidateFeatures();
        }

        private bool ValidateFeatures()
        {
            if (CameraForm.Features == null || !CameraForm.Features.Any())
            {
                featureError = "Selecione pelo menos uma funcionalidade";
                success = false;
            }

            featureError = null;
            return true;
        }
        #endregion


        private void BackToRegister()
        {
            apiResponse = null;
        }

        private async Task Submit()
        {
            //if (!ValidateFeatures())
            //    return;

            visible = true;
            apiResponse = await _cameraService.UpdateAsync(CameraForm);
            visible = false;

            if (apiResponse.Success)
            {
                Snackbar.Add("Camera atualizada com sucesso!", Severity.Success);
                success = false;

                //var result = await _cameraService.GetById(camera.Id);

                //if (result.Success && result is not null && result.Data is not null)
                //{
                //    camera = result.Data;
                //}

                _refreshToken = Guid.NewGuid();
            }
            else
            {
                Snackbar.Add("Houve um erro ao cadastrar a camera, tente novamente mais tarde", Severity.Error);
            }
        }

    }
}
