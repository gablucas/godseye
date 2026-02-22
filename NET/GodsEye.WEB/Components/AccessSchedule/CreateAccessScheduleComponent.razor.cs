using GodsEye.Application.DTOs.Model;
using GodsEye.Application.UseCases.AccessSchedule.Commands.CreateAccessSchedule;
using GodsEye.Domain.Enums;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;


namespace GodsEye.WEB.Components.AccessSchedule
{
    public partial class CreateAccessScheduleComponent
    {
        #region DI

        [Inject]
        AccessScheduleWebService AccessScheduleWebService { get; set; }

        #endregion


        #region PARAMETERS

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        #endregion

        [Parameter]
        public int Id { get; set; }

        #region FORM

        MudForm form;
        private AccessScheduleModel _accessScheduleForm { get; set; } = new();
        private bool success;
        private string[] errors = { };
        private bool visible = false;

        private Dictionary<int, List<string>> rulesError = new();

        #endregion

        protected override async Task OnParametersSetAsync()
        {

            var result = await AccessScheduleWebService.GetById(Id);

            if (result.Success && result is not null && result.Data is not null)
            {
                _accessScheduleForm = result.Data;
            }

            base.OnParametersSet();
        }

        private void OnTimeChanged(TimeSpan? value, AccessScheduleRuleModel time, string timeType, WeekDayEnum dayEnum)
        {
            if (value is null)
                return;

            rulesError = new();

            var newTime = value.Value;

            if (timeType == "start")
                time.StartTime = newTime;
            else
                time.EndTime = newTime;


            var selectedDay = _accessScheduleForm.Rules.Where(x => x.WeekDay == dayEnum).ToList();

            foreach (var (rule, index) in selectedDay.Select((rule, index) => (rule, index)))
            {
               
                if (rule.StartTime > rule.EndTime || rule.EndTime < rule.StartTime || (rule.StartTime == TimeSpan.Zero && rule.EndTime == TimeSpan.Zero))
                {
                    rulesError.Add(index, new List<string> { "start", "end" });
                }


                if (index > 0)
                {
                    if (rule.StartTime <= selectedDay[index - 1].EndTime)
                    {
                        rulesError.Add(index, new List<string> { "start" });
                        rulesError.Add(index - 1, new List<string> { "end" });
                    }
                }
            } 
        }

        private bool HasError(int index, string ruleType)
        {
            if (rulesError.TryGetValue(index, out var value))
            {
                if (value.Contains(ruleType))
                    return true;
                else
                    return false;
            }

            return false;
        }

        private void AddNewRule(WeekDayEnum day)
        {

            if (!_accessScheduleForm.Rules.Any(x => x.WeekDay == day && x.StartTime == TimeSpan.Zero && x.EndTime == TimeSpan.Zero))
            {
                _accessScheduleForm.Rules.Add(
                new AccessScheduleRuleModel()
                {
                    WeekDay = day,
                    StartTime = null,
                    EndTime = null
                });
            }
        }

        private void RemoveRule(WeekDayEnum day, AccessScheduleRuleModel rule)
        {
            _accessScheduleForm.Rules.Remove(rule);
        }

        private async Task Submit()
        {
            
            var createRequest = new CreateAccessScheduleRequest(_accessScheduleForm.Id, _accessScheduleForm.Name, true, _accessScheduleForm.Rules);
            var createResult = await AccessScheduleWebService.CreateAsync(createRequest);

            if (createResult.Success)
            {
                Snackbar.Add("Calendário salvo com sucesso.", Severity.Success);
                MudDialog.Close(DialogResult.Ok(createResult.Data));
            }
            else
            {
                Snackbar.Add("Houve um erro ao criar o calendário.", Severity.Error);
            }

            
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
