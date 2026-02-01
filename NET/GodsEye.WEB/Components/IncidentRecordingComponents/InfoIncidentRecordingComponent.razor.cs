using GodsEye.Application.DTOs.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace GodsEye.WEB.Components.IncidentRecordingComponents
{
    public partial class InfoIncidentRecordingComponent
    {
        [Inject]
        public IJSRuntime JS { get; set; }

        [Parameter]
        public IncidentRecordingModel IncidentRecording { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }


        private void Submit() => MudDialog.Close(DialogResult.Ok(true));

        private void Cancel() => MudDialog.Cancel();

        private async void SetTime(double time)
        {
            await JS.InvokeVoidAsync("goto", time);
        }
    }
}
