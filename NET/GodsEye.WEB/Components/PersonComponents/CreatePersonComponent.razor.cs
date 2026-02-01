using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
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
        PersonService personService { get; set; }

        [Inject]
        SectorWebService sectorService { get; set; }

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

        IEnumerable<SectorModel> _sectors = Enumerable.Empty<SectorModel>();
        

        #endregion

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (shouldStartCamera)
            {
                shouldStartCamera = false;
                await JS.InvokeVoidAsync("cameraFunctions.startCamera");
            }

            var response = await sectorService.GetAllAsync();
            if (response is not null && response.Success)
            {
                _sectors = response.Data;
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

        private string GetSelectedSectorsName(List<string> ids)
        {
            var names = _sectors
                .Where(c => ids.Contains(c.Id.ToString()))
                .Select(c => c.Name);

            return string.Join(", ", names);
        }


        private async Task Submit()
        {
            visible = true;
            apiResponse = await personService.CreateAsync(CreatePersonForm);
            visible = false;

            if (!apiResponse.Success)
                Snackbar.Add("Houve um erro ao cadastrar a pessoa, tente novamente mais tarde", Severity.Error);
            else
                Snackbar.Add("Pessoa cadastrada com sucesso!", Severity.Success);
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
