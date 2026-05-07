
using GodsEye.Shared.Response.Sector;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace GodsEye.WEB.Components.CameraComponents
{
    public partial class CreateCameraComponent
    {
        #region DI

        [Inject]
        CameraWebService CameraService { get; set; }

        [Inject]
        SectorWebService SectorService { get; set; }

        [Inject]
        MediaMtxWebService MediaMtxWebService { get; set; }

        [Inject]
        DialogWebService DialogWebService { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        #endregion

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public int Id { get; set; }

        #region FORM

        MudForm form;
        private bool success;
        private string[] errors = { };
        public CreateCameraForm CameraForm { get; set; } = new();
        IEnumerable<SectorResponse> _sectors = Enumerable.Empty<SectorResponse>();

        private bool _hasConnectionError = false;
        private string? _connectionErrorMessage = null;
        private bool _loadingConnection = false;

        int? apiResponse { get; set; } = null;

        private bool visible = false;
        private bool _videoExpanded = false;

        #endregion


        protected override async Task OnInitializedAsync()
        {
            var response = await SectorService.GetAllAsync();
            if (response is not null)
            {
                _sectors = response;
            }
        }


        private void BackToRegister()
        {
            apiResponse = null;
        }

        private async Task Submit()
        {
            visible = true;
            apiResponse = await CameraService.CreateAsync(CameraForm);
            visible = false;

            if (apiResponse > 0)
            {
                Snackbar.Add("camera cadastrada com Success!", Severity.Success);
                MudDialog.Close(DialogResult.Ok(apiResponse));
            }
            else
            {
                Snackbar.Add("Houve um erro ao cadastrar a camera, tente novamente mais tarde", Severity.Error);
            }   
        }

        private void Cancel() => MudDialog.Cancel();

        private void OnConnectionChanged(string value)
        {
            CameraForm.Connection = value;
            _hasConnectionError = false;
            _connectionErrorMessage = null;
        }


        public async Task StartStream()
        {
            _loadingConnection = true;

            if (string.IsNullOrEmpty(CameraForm.Connection))
            {
                _loadingConnection = false;
                return;
            }
                

            var cam = await MediaMtxWebService.StartStream(CameraForm.Connection);

            if (cam is null)
            {
                _hasConnectionError = true;
                _connectionErrorMessage = "Hou um erro ao se conectar a camera";
                _loadingConnection = false;
                return;
            }
                
            var webRtcUrl = cam;
            _videoExpanded = true;

            await JS.InvokeVoidAsync("streamFunctions.start", "camera-player", webRtcUrl);

            _loadingConnection = false;
            _hasConnectionError = false;
            _connectionErrorMessage = null;
        }

        public async ValueTask DisposeAsync()
        {
            await JS.InvokeVoidAsync("streamFunctions.stop", "camera-player");
        }

        private async Task CreateNewSectorCallback(int sectorId)
        {
            var newSector = await SectorService.GetById(sectorId);

            if (newSector is not null)
            {
                CameraForm.SectorId = sectorId;
                _sectors = _sectors.Append(newSector).ToList();
                StateHasChanged();
            }
        }
    }
}
