using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;

namespace GodsEye.WEB.Components
{
    public partial class CameraConfigurationBarComponent
    {
        [Inject]
        public CameraWebService cameraService { get; set;  }

        [Parameter]
        public int Id { get; set; }

        [Parameter]
        public Guid RefreshToken { get; set; }

        [Parameter]
        public string Pagina { get; set; }

        private List<CameraFeatureModel> _cameraFeatures = new();

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
