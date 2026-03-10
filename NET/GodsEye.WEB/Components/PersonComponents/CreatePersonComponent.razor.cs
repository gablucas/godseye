using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.Domain.Enums;
using GodsEye.WEB.Enum;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;

namespace GodsEye.WEB.Components.PersonComponents
{
    public partial class CreatePersonComponent
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

        [Inject]
        IJSRuntime JS { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        #endregion

        #region FORM

        MudForm form;
        private bool success;
        private string[] errors = { };
        public CreatePersonForm CreatePersonForm { get; set; } = new();

        private IBrowserFile? _file;
        public string? PreviewImage { get; set; } = null;

        private string? CapturedImage;

        PhotoCaptureMethodEnum? photoMethod { get; set; } = null;
        private bool shouldStartCamera;

        ApiResponse<ProcedureResult?>? apiResponse { get; set; } = null;

        private bool visible = false;

        IEnumerable<SectorModel> _sectors = new List<SectorModel>();
        IEnumerable<AccessLevelModel> _accessLevels = Enumerable.Empty<AccessLevelModel>();

        private bool _sectorError = false;
        private string _sectorErrorMessage = "";

        #endregion

        private string _errorMessage = "";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (shouldStartCamera)
            {
                shouldStartCamera = false;
                await JS.InvokeVoidAsync("cameraFunctions.startCamera");
            }

            var response = await SectorService.GetAllAsync();
            if (response is not null && response.Success)
            {
                _sectors = response.Data;
            }

            var accessLevelResponse = await AccessLevelService.GetAllAsync();
            if (accessLevelResponse is not null && accessLevelResponse.Success)
            {
                _accessLevels = accessLevelResponse.Data;
            }

        }

        public void OnSectorChanged(int? sectorId)
        {
            CreatePersonForm.SectorId = sectorId;

            ValidateSectorAccessLevel();
        }

        public void OnAccessLevelChanged(int? accessLevelId)
        {
            CreatePersonForm.AcessLevelId = accessLevelId;
            ValidateSectorAccessLevel();
        }

        private void ValidateSectorAccessLevel()
        {
            _sectorError = false;
            _sectorErrorMessage = "";

            if (CreatePersonForm.AcessLevelId is not null && CreatePersonForm.SectorId is not null)
            {
                var selectedAccessLevel = _accessLevels.First(x => x.Id == CreatePersonForm.AcessLevelId);

                if (!selectedAccessLevel.Sectors.Any(x => x.Id == CreatePersonForm.SectorId))
                {
                    _sectorError = true;
                    _sectorErrorMessage = "O setor selecionado para a pessoa está como não permitido no nível de acesso";
                    Snackbar.Add(_sectorErrorMessage, Severity.Error);
                    return;
                }

                if (selectedAccessLevel.Sectors.Any(x => x.RuleType == AccessLevelSectorRuleEnum.BLACKLIST && x.Id == CreatePersonForm.SectorId))
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

            if (newSector.Success)
            {
                CreatePersonForm.SectorId = sectorId;
                _sectors = _sectors.Append(newSector.Data).ToList();
                StateHasChanged();
            }
        }

        private async Task CreateNewAccessLevelCallback(int accessLevelId)
        {
            var newSector = await AccessLevelService.GetById(accessLevelId);

            if (newSector.Success)
            {
                CreatePersonForm.AcessLevelId = accessLevelId;
                _accessLevels = _accessLevels.Append(newSector.Data).ToList();
                StateHasChanged();
            }
        }

        private async Task OpenCamera(EventArgs e)
        {
            photoMethod = PhotoCaptureMethodEnum.CAMERA;
            shouldStartCamera = true;
            StateHasChanged();
        }

        private async Task TakePhoto()
        {
            PreviewImage = await JS.InvokeAsync<string>("cameraFunctions.capturePhoto");
            CreatePersonForm.Photo = PreviewImage;
        }

        private void BackToRegister()
        {
            PreviewImage = null;
            _file = null;
            CapturedImage = null;
            photoMethod = null;
            CreatePersonForm.Photo = null;
            apiResponse = null;
        }


        private async Task UploadFile(IBrowserFile file)
        {
            _file = file;

            using var stream = _file.OpenReadStream(maxAllowedSize: 5_000_000);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            var bytes = ms.ToArray();
            var base64 = Convert.ToBase64String(bytes);
            var dataUrl = $"data:{file.ContentType};base64,{base64}";

            PreviewImage = dataUrl;
            CreatePersonForm.Photo = dataUrl;
        }


        private async Task Submit()
        {
            visible = true;
            apiResponse = await PersonService.CreateAsync(CreatePersonForm);
            visible = false;

            if (apiResponse.Success)
            {
                Snackbar.Add("Pessoa cadastrada com sucesso!", Severity.Success);
                MudDialog.Close(DialogResult.Ok(apiResponse.Data.Id));
            }
            else
            {
                _errorMessage = apiResponse?.Error?.Message ?? "Houve um erro ao cadastrar a pessoa, tente novamente mais tarde";
                Snackbar.Add(_errorMessage, Severity.Error);
            }
                
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
