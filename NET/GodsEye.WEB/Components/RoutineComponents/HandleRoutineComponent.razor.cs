using AutoMapper;
using GodsEye.Application.DTOs.Model;
using GodsEye.Application.UseCases.Routine.Commands.CreateRoutine;
using GodsEye.Domain.Enums;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.RoutineComponents
{
    public partial class HandleRoutineComponent
    {
        #region DI

        [Inject]
        SectorWebService SectorService { get; set; }

        [Inject]
        RoutineWebService RoutineWebService { get; set; }

        [Inject]
        DialogWebService DialogWebService { get; set; }

        [Inject]
        IMapper Mapper { get; set; }

        #endregion

        #region FORM

        MudForm form;

        RoutineForm RoutineForm { get; set; } = new();

        bool success;
        string[] errors = { };
        bool visible = false;

        protected void AddNewRule()
        {
            switch (RoutineForm.RuleType)
            {
                case RoutineRuleTypeEnum.SectorTransition:
                    RoutineForm.AddNewRule();
                    break;
            }
        }
        
        protected void ChangeRulePosition(RoutineRuleSectorTransitionForm rule, string type)
        {
            var index = RoutineForm.Rules.IndexOf(rule);

            if (type == "UP" && index > 0)
            {
                (RoutineForm.Rules[index], RoutineForm.Rules[index - 1]) =
                (RoutineForm.Rules[index - 1], RoutineForm.Rules[index]);
            }

            if (type == "DOWN" && index < RoutineForm.Rules.Count - 1)
            {
                (RoutineForm.Rules[index], RoutineForm.Rules[index + 1]) =
                (RoutineForm.Rules[index + 1], RoutineForm.Rules[index]);
            }

            ReorderRules();
        }

        protected void DeleteRule(RoutineRuleSectorTransitionForm rule)
        {
            RoutineForm.Rules.Remove(rule);
            ReorderRules();
        }

        private void ReorderRules()
        {
            for (int i = 0; i < RoutineForm.Rules.Count; i++)
            {
                RoutineForm.Rules[i].OrderIndex = i + 1;
            }
        }

        private bool ValidateForm()
        {
            if (RoutineForm.Rules.Count() == 0)
            {
                Snackbar.Add("É preciso ter pelo menos uma regra adicionada", Severity.Error);
                return false;
            }

            return true;
        }

        #endregion

        #region PARAMS

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public int Id { get; set; }

        #endregion

        #region LIFETIME FUNCTIONS

        protected override async Task OnInitializedAsync()
        {
            var sectorsResponse = await SectorService.GetAllAsync();
            if (sectorsResponse is not null && sectorsResponse.Success)
            {
                _sectors = sectorsResponse.Data;
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            var routine = await RoutineWebService.GetById(Id);

            if (routine is null)
            {
                Snackbar.Add("Não foi possível buscar a rotina", Severity.Error);
                MudDialog.Cancel();
            }

            RoutineForm = Mapper.Map<RoutineForm>(routine);
        }

        #endregion

        #region SECTOR

        IEnumerable<SectorModel> _sectors = Enumerable.Empty<SectorModel>();

        public string ValidateSector(RoutineRuleSectorTransitionForm item, int? value)
        {
            if (value == null)
                return "Selecione um setor";

            bool alreadyExists = RoutineForm.Rules
                .Where(x => x != item)
                .Any(x => x.SectorId == value);

            if (alreadyExists)
                return "Esse setor já foi selecionado!";

            return null;
        }

        #endregion

        private async Task Submit()
        {

            await form.Validate();

            if (!ValidateForm())
                return;

            var request = new CreateRoutineRequest
            {
                Id = RoutineForm.Id ?? 0,
                Name = RoutineForm.Name,
                RuleType = RoutineForm.RuleType,
                Rules = RoutineForm.Rules.Select(x => new CreateRoutineRuleDTO
                {
                    OrderIndex = x.OrderIndex,
                    MinTime = x.MinTime,
                    MaxTime = x.MaxTime,
                    SectorId = x.SectorId
                }).ToList()
            };

            visible = true;
            var result = await RoutineWebService.CreateAsync(request);
            visible = false;

            if (result.Success)
            {
                Snackbar.Add("Rotina cadastrada com sucesso!", Severity.Success);
                MudDialog.Close(DialogResult.Ok(result.Data));
            }
            else
            {
                Snackbar.Add("Houve um erro ao cadastrar a rotina, tente novamente mais tarde", Severity.Error);
            }
            

        }

        private void Cancel() => MudDialog.Cancel();
    }
}
