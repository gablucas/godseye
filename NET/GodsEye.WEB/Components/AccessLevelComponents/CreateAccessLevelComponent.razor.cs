using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace GodsEye.WEB.Components.AccessLevelComponents
{
    public partial class CreateAccessLevelComponent
    {
        #region DI

        [Inject]
        AccessScheduleWebService AccessScheduleWebService { get; set; }

        [Inject]
        SectorWebService SectorWebService { get; set; }

        [Inject]
        IJSRuntime JS { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        #endregion

        #region FORM

        MudForm form;
        private bool success;
        private string[] errors = { };
        public AccessLevelForm AccessLevelForm { get; set; } = new();

        private bool shouldStartCamera;

        ApiResponse<ProcedureResult?>? apiResponse { get; set; } = null;

        private bool visible = false;

        IEnumerable<SectorModel> _sectors = Enumerable.Empty<SectorModel>();
        IEnumerable<AccessScheduleModel> _accessSchedule = Enumerable.Empty<AccessScheduleModel>();

        #endregion

        private string _errorMessage = "";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (shouldStartCamera)
            {
                shouldStartCamera = false;
                await JS.InvokeVoidAsync("cameraFunctions.startCamera");
            }

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

        private string GetSelectedSectorsName(List<string> ids)
        {
            var names = _sectors
                .Where(c => ids.Contains(c.Id.ToString()))
                .Select(c => c.Name);

            return string.Join(", ", names);
        }


        private async Task Submit()
        {
            //visible = true;
            //apiResponse = await personService.CreateAsync(AccessLevelForm);
            //visible = false;

            //if (apiResponse.Success)
            //{
            //    Snackbar.Add("Pessoa cadastrada com sucesso!", Severity.Success);
            //    MudDialog.Close(DialogResult.Ok(apiResponse.Data.Id));
            //}
            //else
            //{
            //    _errorMessage = apiResponse?.Error?.Message ?? "Houve um erro ao cadastrar a pessoa, tente novamente mais tarde";
            //    Snackbar.Add(_errorMessage, Severity.Error);
            //}
                
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
