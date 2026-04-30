using AutoMapper;

using GodsEye.Shared.Enums;
using GodsEye.Shared.Response.Sector;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.Compliance.Rules
{
    public partial class SectorTransitionRule
    {
        #region DI

        [Inject]
        SectorWebService SectorService { get; set; }

        [Inject]
        ComplianceWebService ComplianceWebService { get; set; }

        [Inject]
        DialogWebService DialogWebService { get; set; }

        [Inject]
        IMapper Mapper { get; set; }

        #endregion

        protected override async Task OnInitializedAsync()
        {
            var sectorsResponse = await SectorService.GetAllAsync();
            if (sectorsResponse is not null)
            {
                _sectors = sectorsResponse;
            }
        }

        #region PARAMETERS

        #endregion

        #region LIFETIME FUNCTIONS


        #endregion

        #region FORM

        MudForm form;

        ComplianceSectorTransitionRule ComplianceRuleForm { get; set; } = new();

        bool success;
        string[] errors = { };
        bool visible = false;

        protected void AddNewRule()
        {
            switch (ComplianceRuleForm.RuleType)
            {
                case CompliancePolicyEnum.SECTOR_TRANSITION:
                    ComplianceRuleForm.AddNewRule();
                    break;
            }
        }

        protected void ChangeRulePosition(RoutineRuleSectorTransitionForm rule, string type)
        {
            var index = ComplianceRuleForm.Rules.IndexOf(rule);

            if (type == "UP" && index > 0)
            {
                (ComplianceRuleForm.Rules[index], ComplianceRuleForm.Rules[index - 1]) =
                (ComplianceRuleForm.Rules[index - 1], ComplianceRuleForm.Rules[index]);
            }

            if (type == "DOWN" && index < ComplianceRuleForm.Rules.Count - 1)
            {
                (ComplianceRuleForm.Rules[index], ComplianceRuleForm.Rules[index + 1]) =
                (ComplianceRuleForm.Rules[index + 1], ComplianceRuleForm.Rules[index]);
            }

            ReorderRules();
        }

        protected void DeleteRule(RoutineRuleSectorTransitionForm rule)
        {
            ComplianceRuleForm.Rules.Remove(rule);
            ReorderRules();
        }

        private void ReorderRules()
        {
            for (int i = 0; i < ComplianceRuleForm.Rules.Count; i++)
            {
                ComplianceRuleForm.Rules[i].OrderIndex = i + 1;
            }
        }

        private bool ValidateForm()
        {
            if (ComplianceRuleForm.Rules.Count() == 0)
            {
                Snackbar.Add("É preciso ter pelo menos uma regra adicionada", Severity.Error);
                return false;
            }

            return true;
        }

        #endregion

        #region SECTOR

        IEnumerable<SectorResponse> _sectors = Enumerable.Empty<SectorResponse>();

        public string ValidateSector(RoutineRuleSectorTransitionForm item, int? value)
        {
            if (value == null)
                return "Selecione um setor";

            bool alreadyExists = ComplianceRuleForm.Rules
                .Where(x => x != item)
                .Any(x => x.SectorId == value);

            if (alreadyExists)
                return "Esse setor já foi selecionado!";

            return null;
        }

        #endregion


        protected async Task Submit()
        {

            await form.Validate();

            if (!ValidateForm())
                return;

            var request = new ComplianceSectorTransitionRule
            {
                Id = ComplianceRuleForm.Id ?? 0,
                Name = ComplianceRuleForm.Name,
                Rules = ComplianceRuleForm.Rules.Select(x => new RoutineRuleSectorTransitionForm
                {
                    OrderIndex = x.OrderIndex,
                    MinTime = x.MinTime,
                    MaxTime = x.MaxTime,
                    SectorId = x.SectorId
                }).ToList()
            };

            visible = true;
            var result = await ComplianceWebService.CreateAsync(request);
            visible = false;

            if (result > 0)
            {
                Snackbar.Add("Rotina cadastrada com sucesso!", Severity.Success);
                //MudDialog.Close(DialogResult.Ok(result.Data));
            }
            else
            {
                Snackbar.Add("Houve um erro ao cadastrar a rotina, tente novamente mais tarde", Severity.Error);
            }
        }
    }

}
