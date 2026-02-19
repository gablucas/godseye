using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;


namespace GodsEye.WEB.Components.DwellTimeMonitoringComponents
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

        }

        #region TIMER

        private PeriodicTimer _timer;
        private DateTime _now;

        protected override void OnInitialized()
        {
            _now = DateTime.Now;
            _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            //_ = StartTimer();
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
            _ = StartTimer();

            var ts = _now - start;
            return $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
        }

        private string GetDiffHours(DateTime enteredAt, DateTime exitedAt)
        {
            var ts = exitedAt - enteredAt;

            var hours = (int)ts.TotalHours;
            return $"{hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
        }

        #endregion

        

    }
}
