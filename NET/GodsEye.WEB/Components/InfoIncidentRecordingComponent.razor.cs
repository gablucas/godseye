using GodsEye.Application.DTOs.Model;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components
{
    public partial class InfoIncidentRecordingComponent
    {
        [Parameter]
        public IncidentRecordingModel IncidentRecording { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }


        private void Submit() => MudDialog.Close(DialogResult.Ok(true));

        private void Cancel() => MudDialog.Cancel();
    }
}
