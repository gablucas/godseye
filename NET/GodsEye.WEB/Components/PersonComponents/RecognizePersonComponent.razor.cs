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
    public partial class RecognizePersonComponent
    {
        #region DI

        [Inject]
        PersonService PersonService { get; set; }

        [Inject]
        IJSRuntime JS { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        #endregion

        #region PARAMETERS

        [Parameter]
        public int PersonId { get; set; }

        #endregion

        #region FORM

        MudForm form;
        private bool success;
        private string[] errors = { };

        public PersonRecognizeForm PersonRecognizeForm { get; set; } = new();

        private IBrowserFile? _file;
        public string? PreviewImage { get; set; } = null;

        private string? CapturedImage;

        PhotoCaptureMethodEnum? photoMethod { get; set; } = null;
        private bool shouldStartCamera;

        ProcedureResult? apiResponse { get; set; } = null;

        private bool visible = false;

        #endregion

        private string _errorMessage = "";

        protected override async Task OnParametersSetAsync()
        {
            PersonRecognizeForm.PersonId = PersonId;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (shouldStartCamera)
            {
                shouldStartCamera = false;
                await JS.InvokeVoidAsync("cameraFunctions.startCamera");
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
            PersonRecognizeForm.Photo = PreviewImage;
        }

        private void BackToRegister()
        {
            PreviewImage = null;
            _file = null;
            CapturedImage = null;
            photoMethod = null;
            PersonRecognizeForm.Photo = "";
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
            PersonRecognizeForm.Photo = dataUrl;
        }


        private async Task Submit()
        {
            visible = true;
            apiResponse = await PersonService.CreateRecognizeAsync(PersonRecognizeForm);
            visible = false;

            if (apiResponse is not null)
            {
                Snackbar.Add("Pessoa cadastrada com sucesso!", Severity.Success);
                MudDialog.Close(DialogResult.Ok(apiResponse.Id));
            }
            else
            {
                _errorMessage = "Houve um erro ao cadastrar a pessoa, tente novamente mais tarde";
                Snackbar.Add(_errorMessage, Severity.Error);
            }

        }

        private void Cancel() => MudDialog.Cancel();
    }
}
