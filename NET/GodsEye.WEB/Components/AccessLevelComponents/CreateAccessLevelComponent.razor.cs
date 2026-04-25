using GodsEye.Application.DTOs.Response;
using GodsEye.Application.UseCases.AccessLevel.Commands.CreateOrUpdateAccessLevel;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Shared.Enums;
using GodsEye.Shared.Response.AccessSchedule;
using GodsEye.Shared.Response.Sector;
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

        [Inject]
        DialogWebService DialogWebService { get; set; }

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

        private bool _multiselectionTextChoice;
        private bool _multiselectionTextChoiceBlackList;

        ApiResponse<ProcedureResult?>? apiResponse { get; set; } = null;

        private bool visible = false;

        IEnumerable<SectorResponse> _sectors = Enumerable.Empty<SectorResponse>();
        IEnumerable<AccessScheduleResponse> _accessSchedule = Enumerable.Empty<AccessScheduleResponse>();

        List<SectorResponse> NotAllowed = new();

        #endregion

        private string _errorMessage = "";

        protected override async Task OnParametersSetAsync()
        {
            if (Id != 0)
            {
                var accessLevelResult = await AccessLevelWebService.GetById(Id);

                if (accessLevelResult is not null)
                {
                    AccessLevelForm = new CreateOrUpdateAccessLevelRequest()
                    {
                        Id = accessLevelResult.Id,
                        Name = accessLevelResult.Name,
                        Sectors = accessLevelResult.Sectors.Select(x => new SectorAccessLevelInput(x.Id, x.RuleType)).ToList(),
                        AccessScheduleId = accessLevelResult.SectorSchedule.Id
                    };
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            var accessScheduleResult = await AccessScheduleWebService.GetAllAsync();
            if (accessScheduleResult is not null)
            {
                _accessSchedule = accessScheduleResult.ToList(); 
            }

            var sectorResult = await SectorWebService.GetAllAsync();
            if (sectorResult is not null)
            {
                _sectors = sectorResult.ToList();
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

        private async Task CreateNewAccessScheduleCallback(int accessScheduleId)
        {
            var newAccessSchedule = await AccessScheduleWebService.GetById(accessScheduleId);

            if (newAccessSchedule is not null)
            {
                AccessLevelForm.AccessScheduleId = accessScheduleId;
                _accessSchedule = _accessSchedule.Append(newAccessSchedule).ToList();
                StateHasChanged();
            }
        }

        private async Task Submit()
        {
            var result = await AccessLevelWebService.CreateOrUpdateAsync(AccessLevelForm);

            if (result > 0)
            {
                Snackbar.Add($"Nível de acesso {(AccessLevelForm.Id == 0 ? "criado" : "atualizado")} com sucesso.", Severity.Success);
                MudDialog.Close(DialogResult.Ok(result));
            }
            else
            {
                Snackbar.Add("Houve um erro ao criar o nível de acesso.", Severity.Error);
            }
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
