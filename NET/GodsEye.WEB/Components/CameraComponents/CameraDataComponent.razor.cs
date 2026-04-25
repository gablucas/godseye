using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Shared.Response.Camera;
using GodsEye.Shared.Response.Sector;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.CameraComponents
{
    public partial class CameraDataComponent
    {
        #region DI

        [Inject]
        CameraWebService CameraWebService { get; set; }

        [Inject]
        SectorWebService SectorWebService { get; set; }

        [Inject]
        FeatureWebService FeatureWebService { get; set; }

        #endregion

        #region PARAMS

        [Parameter]
        public int Id { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        #endregion

        #region FORM

        MudForm form;
        UpdateCameraForm CameraForm { get; set; } = new();
        IEnumerable<SectorResponse> _sectors = Enumerable.Empty<SectorResponse>();
        IEnumerable<FeatureModel> _features = Enumerable.Empty<FeatureModel>();

        private bool success;
        private string[] errors = { };


        #endregion

        public CameraResponse camera { get; set; }

        int? apiResponse { get; set; } = null;

        private bool visible = false;

        Guid _refreshToken = Guid.NewGuid();

        protected override async Task OnParametersSetAsync()
        {

            var result = await CameraWebService.GetById(Id);

            if (result is not null)
            {
                camera = result;

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
            var response = await SectorWebService.GetAllAsync();
            if (response is not null)
            {
                _sectors = response;
            }

            var featureResponse = await FeatureWebService.GetAllAsync();
            if (featureResponse is not null && featureResponse.Success)
            {
                _features = featureResponse.Data.ToList();
            }
        }

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

        

        private async Task Submit()
        {
            visible = true;
            apiResponse = await CameraWebService.UpdateAsync(CameraForm);
            visible = false;

            if (apiResponse > 0)
            {
                Snackbar.Add("Camera atualizada com sucesso!", Severity.Success);
                //MudDialog.Close(DialogResult.Ok(1));
            }
            else
            {
                Snackbar.Add("Houve um erro ao cadastrar o setor, tente novamente mais tarde", Severity.Error);
            }

        }

        private void Cancel() => MudDialog.Cancel();
    }
}
