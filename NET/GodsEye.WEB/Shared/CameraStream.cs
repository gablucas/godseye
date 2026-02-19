using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GodsEye.WEB.Shared
{
    public class CameraStream : ComponentBase
    {
        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public MediaMtxWebService MediaMtxWebService { get; set; }

        public bool _loadingConnection = false;
        public bool _hasConnectionError = false;
        public string? _connectionErrorMessage = null;

        public bool? _mediaMtxStatus = null;


        public async Task StartStream(string cameraConnection)
        {
            _loadingConnection = true;

            if (string.IsNullOrEmpty(cameraConnection))
            {
                _loadingConnection = false;
                return;
            }

            var isMediaMtxOnline = await MediaMtxWebService.CheckStatus();

            if (!isMediaMtxOnline.Success || !isMediaMtxOnline.Data)
            {
                _mediaMtxStatus = false;
                _loadingConnection = false;
                await InvokeAsync(StateHasChanged);
                return;
            }


            var cam = await MediaMtxWebService.StartStream(cameraConnection);

            if (cam is null || !cam.Success)
            {
                _hasConnectionError = true;
                _connectionErrorMessage = cam.Error.Message;
                _loadingConnection = false;
                return;
            }

            var webRtcUrl = cam.Data;

            await JS.InvokeVoidAsync("streamFunctions.start", "camera-player", webRtcUrl);

            _loadingConnection = false;
            _hasConnectionError = false;
            _connectionErrorMessage = null;
            StateHasChanged();
        }
    }
}
