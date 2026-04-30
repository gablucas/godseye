
using GodsEye.Shared.Response.Camera;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.CameraComponents
{
    public partial class CameraConfigurationBarComponent
    {
        [Inject]
        public CameraWebService cameraService { get; set;  }

        [Inject]
        public DialogWebService dialogWebService { get; set; }

        [Parameter]
        public Guid RefreshToken { get; set; }

        [Parameter]
        public string Pagina { get; set; }

        [Parameter]
        public int Id { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        private List<CameraFeatureResponse> _cameraFeatures = new();

        protected override async Task OnParametersSetAsync()
        {
            var result = await cameraService.GetFeatures(Id);
            _cameraFeatures = result.ToList();
        }

        private bool HasFeature(int featureId)
        {
            return _cameraFeatures.Any(x => x.Id == featureId);
        }
    }
}
