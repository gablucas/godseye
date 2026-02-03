using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.Json;

namespace GodsEye.WEB.Components.PersonComponents
{
    public partial class InfoPersonComponent
    {
        [Inject]
        public EnvironmentMonitoringWebService EnvironmentMonitoringWebService { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public int Id { get; set; }

        #region TABLE PARAMETERS

        private EnvironmentMonitoringPersonModel _personLogs = new();
        private List<EnvironmentMonitoringPersonLog> _logs = new();

        private MudTable<EnvironmentMonitoringPersonLog> _mudTable;
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
            var result = await EnvironmentMonitoringWebService.GetByPersonId(Id);

            if (result.Success)
                _personLogs = result.Data;

                if (_personLogs.Logs.Count() > 0)
                {
                    _logs = _personLogs.Logs.OrderByDescending(x => x.CreatedAt).ToList();
                    CalculateTimeOnSector();
                }
        }
    }
}
