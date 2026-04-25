using GodsEye.Application.DTOs.Model;
using GodsEye.Application.UseCases.AccessSchedule.Commands.CreateAccessSchedule;
using GodsEye.Shared.Enums;
using GodsEye.Shared.Response.AccessSchedule;
using GodsEye.WEB.Helpers;
using GodsEye.WEB.Model.Forms;
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
        private AccessScheduleForm _accessScheduleForm { get; set; } = new();
        private bool success;
        private string[] errors = { };
        private bool visible = false;

        private List<TimeRuleErrorHelper> _timeRuleErrors = new();

        private List<WeekDayEnum> _selectedWeekDays = new();

        #endregion

        protected override async Task OnParametersSetAsync()
        {
            if (Id != 0)
            {
                var result = await AccessScheduleWebService.GetById(Id);

                if (result is not null)
                {
                    _accessScheduleForm.Id = result.Id;
                    _accessScheduleForm.Name = result.Name;
                    _accessScheduleForm.IsActive = result.IsActive;
                    _accessScheduleForm.Rules = result.Rules.Select(x => new ScheduleRuleDTO() { Id = x.Id, WeekDay = x.WeekDay, StartTime = x.StartTime, EndTime = x.EndTime }).ToList();

                    _selectedWeekDays = _accessScheduleForm.Rules.Select(x => x.WeekDay).Distinct().ToList();
                }
            }
        }

        private void ToogleWeekDay(WeekDayEnum day)
        {
            if (_selectedWeekDays.Contains(day))
            {
                _selectedWeekDays.Remove(day);
            }
            else
            {
                _selectedWeekDays.Add(day);
                AddNewRule(day);
            }
        }

        private void OnTimeChanged(TimeSpan? value, ScheduleRuleDTO time, WeekDayEnum dayEnum, string timeType)
        {
            if (value is null)
                return;

            var newTime = value.Value;

            if (timeType == "start")
            {
                time.StartTime = newTime;
            }
            else
            {
                time.EndTime = newTime;
                _accessScheduleForm.Rules = _accessScheduleForm.Rules.OrderBy(rule => rule.StartTime).ThenBy(rule => rule.EndTime).ToList();
                ValidateTimeRules();
            }
        }

        private void CopyAccessSchedule(WeekDayEnum dayPaste, WeekDayEnum dayCopy)
        {
            var copies = _accessScheduleForm.Rules
                .Where(x => x.WeekDay == dayCopy)
                .Select(x => new ScheduleRuleDTO
                {
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    WeekDay = dayPaste
                })
                .ToList();


            _accessScheduleForm.Rules.RemoveAll(x => x.WeekDay == dayPaste && x.StartTime == TimeSpan.Zero && x.EndTime == TimeSpan.Zero);
            _accessScheduleForm.Rules.AddRange(copies);
        }

        private bool ValidateTimeRules()
        {
            _timeRuleErrors.Clear();

            foreach (var dayGroup in _accessScheduleForm.Rules.GroupBy(x => x.WeekDay))
            {
                var selectedDay = dayGroup.ToList();

                foreach (var (rule, index) in selectedDay.Select((rule, index) => (rule, index)))
                {
                    var start = rule.StartTime;
                    var end = rule.EndTime;

                    
                    // 3. Ambos têm valor, agora dá pra comparar
                    if (start > end)
                    {
                        _timeRuleErrors.Add(new TimeRuleErrorHelper(
                            index,
                            rule.WeekDay,
                            true,
                            true,
                            "Hora inicial maior que a hora final"));
                    }
                    else if (start == end)
                    {
                        _timeRuleErrors.Add(new TimeRuleErrorHelper(
                            index,
                            rule.WeekDay,
                            true,
                            true,
                            "Hora inicial igual à hora final"));
                    }
                    // 4. Só aqui checamos conflito com a regra anterior
                    else if (index > 0)
                    {
                        var previousRule = selectedDay[index - 1];

                        // Garante que o anterior tem EndTime
                        if (previousRule.EndTime is not null && start <= previousRule.EndTime)
                        {
                            _timeRuleErrors.Add(new TimeRuleErrorHelper(
                                index - 1,
                                rule.WeekDay,
                                false,
                                true,
                                null));

                            _timeRuleErrors.Add(new TimeRuleErrorHelper(
                                index,
                                rule.WeekDay,
                                true,
                                false,
                                "Hora inicial menor ou igual à hora final anterior"));
                        }
                    }
                }
            }


            if (_timeRuleErrors.Any())
            {
                Snackbar.Add("Há campos de horário com erro", Severity.Error);
                return false;
            }

            return true;
        }

        private bool HasError(int index, WeekDayEnum day, string ruleType)
        {
            return _timeRuleErrors.Any(rule =>
                rule.Day == day &&
                rule.Index == index &&
                (
                    (ruleType == "start" && rule.IsStartTimeWrong) ||
                    (ruleType == "end" && rule.IsEndTimeWrong)
                )
            );
        }

        private void AddNewRule(WeekDayEnum day)
        {
            var selectedDay = _accessScheduleForm.Rules.Where(x => x.WeekDay == day).ToList();

            if (selectedDay.Count() == 0)
            {
                _accessScheduleForm.Rules.Add(
                new ScheduleRuleDTO()
                {
                    WeekDay = day,
                    StartTime = TimeSpan.Zero,
                    EndTime = TimeSpan.Zero
                });
            }
        }

        private void RemoveRule(WeekDayEnum day, ScheduleRuleDTO rule)
        {
            _accessScheduleForm.Rules.Remove(rule);
        }

        private async Task Submit()
        {
            await form.Validate();

            _accessScheduleForm.Rules.RemoveAll(rule => !_selectedWeekDays.Contains(rule.WeekDay));

            if (string.IsNullOrEmpty(_accessScheduleForm.Name))
            {
                Snackbar.Add("Campo de nome vazio", Severity.Error);
                return;
            }

            if (!ValidateTimeRules())
                return;

            var createResult = await AccessScheduleWebService.CreateAsync(_accessScheduleForm);

            if (createResult > 0)
            {
                Snackbar.Add("Calendário salvo com sucesso.", Severity.Success);
                MudDialog.Close(DialogResult.Ok(createResult));
            }
            else
            {
                Snackbar.Add("Houve um erro ao criar o calendário.", Severity.Error);
            }
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
