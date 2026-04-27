using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GodsEye.WEB.Components.StreamCameraComponent
{
    public partial class StreamCameraComponent
    {
        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public MediaMtxWebService MediaMtxWebService { get; set; }

        [Parameter]
        public string Connection { get; set; }

        public bool _loadingConnection = false;
        public bool _hasConnectionError = false;
        public string? _connectionErrorMessage = null;

        public bool? _mediaMtxStatus = null;

        protected override async Task OnParametersSetAsync()
        {
            if (!string.IsNullOrEmpty(Connection))
                _ = StartStream(Connection);
        }

        public async Task StartStream(string cameraConnection)
        {
            _loadingConnection = true;

            if (string.IsNullOrEmpty(cameraConnection))
            {
                _loadingConnection = false;
                return;
            }

            var isMediaMtxOnline = await MediaMtxWebService.CheckStatus();

            if (!isMediaMtxOnline)
            {
                _mediaMtxStatus = false;
                _loadingConnection = false;
                await InvokeAsync(StateHasChanged);
                return;
            }


            var cam = await MediaMtxWebService.StartStream(cameraConnection);

            if (cam is null)
            {
                _hasConnectionError = true;
                _connectionErrorMessage = "Houve um erro ao visualizar a camera";
                _loadingConnection = false;
                return;
            }

            var webRtcUrl = cam;

            await JS.InvokeVoidAsync("streamFunctions.start", "camera-player", webRtcUrl);

            _loadingConnection = false;
            _hasConnectionError = false;
            _connectionErrorMessage = null;
            StateHasChanged();
        }

        public async ValueTask DisposeAsync()
        {
            await JS.InvokeVoidAsync("streamFunctions.stop", "camera-player");
        }
    }
}
