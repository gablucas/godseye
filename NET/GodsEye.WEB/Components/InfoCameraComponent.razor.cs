using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;

namespace GodsEye.WEB.Components
{
    public partial class InfoCameraComponent
    {
        [Inject]
        public GodsEyeWebService GodsEyeWebService { get; set; }

        [Inject]
        public CameraService CameraService { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public CameraModel Camera { get; set; }

        #region TABLE PARAMETERS

        private IEnumerable<CameraLogModel> _log;
        private MudTable<CameraLogModel> _mudTable;
        bool _loading;

        #endregion


        private void Submit() => MudDialog.Close(DialogResult.Ok(true));

        private void Cancel() => MudDialog.Cancel();

        protected override async Task OnParametersSetAsync()
        {
            var result = await CameraService.GetLogs(Camera.Id);

            if (result.Success)
                _log = result.Data;


            var cam = await GodsEyeWebService.StartStream(Camera);

            if (cam.Success)
            {
                string fullUrl = $"http://localhost:8000/api{cam.Data.Url}";
                await JS.InvokeVoidAsync("loadHlsVideo", "camera-player", fullUrl);
            }
        }

    }
}
