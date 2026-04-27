using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Shared.Enums;
using GodsEye.Shared.Response.AccessLevel;
using GodsEye.Shared.Response.Sector;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.PersonComponents
{
    public partial class CreateOrUpdatePersonComponent
    {
        #region DI

        [Inject]
        PersonService PersonService { get; set; }

        [Inject]
        SectorWebService SectorService { get; set; }

        [Inject]
        AccessLevelWebService AccessLevelService { get; set; }

        [Inject]
        DialogWebService DialogWebService { get; set; }


        #endregion

        #region PARAMETERS

        [Parameter]
        public int PersonId { get; set; }

        #endregion

        #region FORM

        MudForm form;
        private bool success;
        private string[] errors = { };
        public PersonForm PersonForm { get; set; } = new();

        ProcedureResult? result { get; set; } = null;

        private bool visible = false;

        IEnumerable<SectorResponse> _sectors = new List<SectorResponse>();
        IEnumerable<AccessLevelResponse> _accessLevels = Enumerable.Empty<AccessLevelResponse>();

        private bool _sectorError = false;
        private string _sectorErrorMessage = "";

        #endregion

        private string _errorMessage = "";


        protected override async Task OnInitializedAsync()
        {
            var response = await SectorService.GetAllAsync();
            if (response is not null)
            {
                _sectors = response;
            }

            var accessLevelResponse = await AccessLevelService.GetAllAsync();
            if (accessLevelResponse is not null)
            {
                _accessLevels = accessLevelResponse;
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            if (PersonId != 0)
            {
                var personResponse = await PersonService.GetById(PersonId);

                if (personResponse is not null)
                    PersonForm = new PersonForm(personResponse);
            }
        }

        public void OnSectorChanged(int? sectorId)
        {
            PersonForm.SectorId = sectorId;

            ValidateSectorAccessLevel();
        }

        public void OnAccessLevelChanged(int? accessLevelId)
        {
            PersonForm.AccessLevelId = accessLevelId;
            ValidateSectorAccessLevel();
        }

        private void ValidateSectorAccessLevel()
        {
            _sectorError = false;
            _sectorErrorMessage = "";

            if (PersonForm.AccessLevelId is not null && PersonForm.SectorId is not null)
            {
                var selectedAccessLevel = _accessLevels.First(x => x.Id == PersonForm.AccessLevelId);

                if (!selectedAccessLevel.Sectors.Any(x => x.Id == PersonForm.SectorId))
                {
                    _sectorError = true;
                    _sectorErrorMessage = "O setor selecionado para a pessoa está como não permitido no nível de acesso";
                    Snackbar.Add(_sectorErrorMessage, Severity.Error);
                    return;
                }

                if (selectedAccessLevel.Sectors.Any(x => x.RuleType == AccessLevelSectorRuleEnum.BLACKLIST && x.Id == PersonForm.SectorId))
                {
                    _sectorError = true;
                    _sectorErrorMessage = "O setor selecionado para a pessoa está na lista negra do nível de acesso";
                    Snackbar.Add(_sectorErrorMessage, Severity.Error);
                    return;
                }
            }
        }

        private async Task CreateNewSectorCallback(int sectorId)
        {
            var newSector = await SectorService.GetById(sectorId);

            if (newSector is not null)
            {
                PersonForm.SectorId = sectorId;
                _sectors = _sectors.Append(newSector).ToList();
                StateHasChanged();
            }
        }

        private async Task CreateNewAccessLevelCallback(int accessLevelId)
        {
            var newSector = await AccessLevelService.GetById(accessLevelId);

            if (newSector is not null)
            {
                PersonForm.AccessLevelId = accessLevelId;
                _accessLevels = _accessLevels.Append(newSector).ToList();
                StateHasChanged();
            }
        }

        private async Task Submit()
        {
            visible = true;

            if (PersonId == 0)
            {
                result = await PersonService.CreateAsync(PersonForm);

                if (result is not null)
                {
                    Snackbar.Add("Pessoa cadastrada com sucesso!", Severity.Success);
                }
                else
                {
                    _errorMessage = "Houve um erro ao cadastrar a pessoa, tente novamente mais tarde";
                    Snackbar.Add(_errorMessage, Severity.Error);
                }
            }
            else
            {
                result = await PersonService.UpdateAsync(PersonForm);

                if (result is not null)
                {
                    Snackbar.Add("Pessoa atualizada com sucesso!", Severity.Success);
                }
                else
                {
                    _errorMessage = "Houve um erro ao atualizar a pessoa, tente novamente mais tarde";
                    Snackbar.Add(_errorMessage, Severity.Error);
                }
            }

            visible = false;
        }
    }
}
