using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;

namespace GodsEye.WEB.Components
{
    public partial class InfoDwellTimeMonitoringComponent
    {
        [Inject]
        public GodsEyeWebService GodsEyeWebService { get; set; }

        [Inject]
        public DwellTimeMonitoringWebService DwellTimeMonitoringService { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public CameraByFeatureModel Camera { get; set; }

        #region TABLE PARAMETERS

        private List<DwellTimeMonitoringDetailsModel> _log;
        bool _loading;

        #endregion


        private void Submit() => MudDialog.Close(DialogResult.Ok(true));

        private void Cancel() => MudDialog.Cancel();

        protected override async Task OnParametersSetAsync()
        {
            var result = await DwellTimeMonitoringService.GetDetailsByCameraId(Camera.Id);

            if (result.Success)
                _log = result.Data.ToList();

            // Só aceita o objeto CameraModel -> Ver se pode alterar somente para o ID da camera
            //var cam = await GodsEyeWebService.StartStream(Camera.id);

            //if (cam.Success)
            //{
            //    string fullUrl = $"http://localhost:8000/api{Camera.Connection}";
            //    await JS.InvokeVoidAsync("loadHlsVideo", "camera-player", fullUrl);
            //}
        }

        #region TIMER

        private PeriodicTimer _timer;
        private DateTime _now;

        protected override void OnInitialized()
        {
            _now = DateTime.Now;
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            _ = StartTimer();
        }

        private async Task StartTimer()
        {
            while (await _timer.WaitForNextTickAsync())
            {
                _now = DateTime.Now;
                await InvokeAsync(StateHasChanged);
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private string GetElapsed(DateTime start)
        {
            var ts = _now - start;
            return $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
        }

        #endregion

    }
}
