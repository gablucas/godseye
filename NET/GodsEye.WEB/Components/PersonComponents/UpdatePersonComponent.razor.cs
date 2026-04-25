using GodsEye.Shared.Response.EnvironmentMonitoring;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.PersonComponents
{
    public partial class UpdatePersonComponent
    {
        [Inject]
        public EnvironmentMonitoringWebService EnvironmentMonitoringWebService { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public int PersonId { get; set; }

        #region TABLE PARAMETERS

        private EnvironmentMonitoringPersonResponse _personLogs = new();
        private List<EnvironmentMonitoringPersonLogResponse> _logs = new();

        private MudTable<EnvironmentMonitoringPersonLogResponse> _mudTable;
        bool _loading;

        #endregion

        private void CalculateTimeOnSector()
        {
            for (int i = 0; i < _logs.Count(); i++)
            {
                if (i < _logs.Count() - 1)
                {
                    var diff = _logs[i].CreatedAt - _logs[i + 1].CreatedAt;

                    // o tempo pertence ao registro MAIS ANTIGO
                    _logs[i + 1].TimeOnSector = diff;
                }
            }

            // o mais novo não tem tempo fechado ainda
            _logs[0].TimeOnSector = null;
        }

        private void Submit() => MudDialog.Close(DialogResult.Ok(true));

        private void Cancel() => MudDialog.Cancel();

        protected override async Task OnParametersSetAsync()
        {
            var result = await EnvironmentMonitoringWebService.GetByPersonId(PersonId);

            if (result is not null)
                _personLogs = result;

                if (_personLogs.Logs.Count() > 0)
                {
                    _logs = _personLogs.Logs.OrderByDescending(x => x.CreatedAt).ToList();
                    CalculateTimeOnSector();
                }
        }
    }
}
