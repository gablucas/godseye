using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.UseCases.AccessLevel.Commands.CreateOrUpdateAccessLevel;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Enums;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;


namespace GodsEye.WEB.Components.AccessLevelComponents
{
    public partial class CreateAccessLevelComponent
    {
        #region DI

        [Inject]
        AccessScheduleWebService AccessScheduleWebService { get; set; }

        [Inject]
        AccessLevelWebService AccessLevelWebService { get; set; }

        [Inject]
        SectorWebService SectorWebService { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        #endregion

        #region PARAMS

        [Parameter]
        public int Id { get; set; }

        #endregion

        #region FORM

        MudForm form;
        private bool success;
        private string[] errors = { };
        public CreateOrUpdateAccessLevelRequest AccessLevelForm { get; set; } = new();

        private bool shouldStartCamera;

        private bool _multiselectionTextChoice;
        private bool _multiselectionTextChoiceBlackList;

        ApiResponse<ProcedureResult?>? apiResponse { get; set; } = null;

        private bool visible = false;

        IEnumerable<SectorModel> _sectors = Enumerable.Empty<SectorModel>();
        IEnumerable<AccessScheduleModel> _accessSchedule = Enumerable.Empty<AccessScheduleModel>();

        List<SectorModel> NotAllowed = new();

        #endregion

        private string _errorMessage = "";

        protected override async Task OnParametersSetAsync()
        {
            if (Id != 0)
            {
                var accessLevelResult = await AccessLevelWebService.GetById(Id);

                if (accessLevelResult.Success && accessLevelResult is not null && accessLevelResult.Data is not null)
                {
                    AccessLevelForm = new CreateOrUpdateAccessLevelRequest()
                    {
                        Id = accessLevelResult.Data.Id,
                        Name = accessLevelResult.Data.Name,
                        Sectors = accessLevelResult.Data.Sectors.Select(x => new SectorAccessLevelInput(x.Id, x.RuleType)).ToList(),
                        AccessScheduleId = accessLevelResult.Data.SectorSchedule.Id,
                    };
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            var accessScheduleResult = await AccessScheduleWebService.GetAllAsync();
            if (accessScheduleResult is not null && accessScheduleResult.Success)
            {
                _accessSchedule = accessScheduleResult.Data;
            }

            var sectorResult = await SectorWebService.GetAllAsync();
            if (sectorResult is not null && sectorResult.Success)
            {
                _sectors = sectorResult.Data;
            }
        }

        private void OnSectorsChanged(IEnumerable<int> values, AccessLevelSectorRuleEnum rule)
        {
            AccessLevelForm.Sectors.RemoveAll(x => x.RuleType == rule);
            var newList = values.Select(id => new SectorAccessLevelInput(id, rule)).ToList();
            AccessLevelForm.Sectors.AddRange(newList);
        }

        private string GetMultiSelectionText(IReadOnlyList<string> selectedValues)
        {
            if (_multiselectionTextChoice)
            {
                return $"Setor{(selectedValues.Count > 1 ? "es selecionados" : " selecionado")}: {string.Join(", ", selectedValues.Select(x => x))}";
            }

            return $"{selectedValues.Count} setor{(selectedValues.Count > 1 ? "es selecionados" : " selecionado")}";
        }

        private string GetMultiSelectionTextBlacklist(IReadOnlyList<string> selectedValues)
        {
            if (_multiselectionTextChoiceBlackList)
            {
                return $"Setor{(selectedValues.Count > 1 ? "es selecionados" : " selecionado")}: {string.Join(", ", selectedValues.Select(x => x))}";
            }

            return $"{selectedValues.Count} setor{(selectedValues.Count > 1 ? "es selecionados" : " selecionado")}";
        }

        private void RemoveSector(int sectorId)
        {
            AccessLevelForm.Sectors = AccessLevelForm.Sectors.Where(x => x.SectorId != sectorId).ToList();
        }


        private async Task Submit()
        {
            var result = await AccessLevelWebService.CreateOrUpdateAsync(AccessLevelForm);

            if (result.Success)
            {
                Snackbar.Add($"Nível de acesso {(AccessLevelForm.Id == 0 ? "criado" : "atualizado")} com sucesso.", Severity.Success);
                MudDialog.Close(DialogResult.Ok(result.Data));
            }
            else
            {
                Snackbar.Add("Houve um erro ao criar o nível de acesso.", Severity.Error);
            }
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
