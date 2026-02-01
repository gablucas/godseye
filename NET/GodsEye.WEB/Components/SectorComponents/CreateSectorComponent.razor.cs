using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components.SectorComponents
{
    public partial class CreateSectorComponent
    {
        #region DI

        [Inject]
        SectorWebService sectorService { get; set; }

        [Inject]
        CameraWebService cameraService { get; set; }

        [Inject]
        NotificationGroupWebService notificationGroupService { get; set; }

        #endregion

        #region FORM

        MudForm form;

        CreateSectorForm CreateSectorForm { get; set; } = new();
        public IEnumerable<string> NotificationGroups { get; set; }
        ApiResponse<ProcedureResult?>? apiResponse { get; set; } = null;

        bool success;
        string[] errors = { };
        bool visible = false;

        #endregion

        IEnumerable<NotificationGroupModel> _notificationGroups = Enumerable.Empty<NotificationGroupModel>();

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var notificationResponse = await notificationGroupService.GetAllAsync();
            if (notificationResponse is not null && notificationResponse.Success)
            {
                _notificationGroups = notificationResponse.Data;
            }
        }

        private void BackToRegister()
        {
            apiResponse = null;
        }

        private string GetSelectNotificationGroupsName(List<string> ids)
        {
            var names = _notificationGroups
                .Where(c => ids.Contains(c.Id.ToString()))
                .Select(c => c.Name);

            return string.Join(", ", names);
        }

        private async Task Submit()
        {
            visible = true;
            apiResponse = await sectorService.CreateAsync(CreateSectorForm);
            visible = false;

            if (apiResponse.Success)
            {
                Snackbar.Add("Setor cadastrado com sucesso!", Severity.Success);
                MudDialog.Close(DialogResult.Ok(1));
            }
            else
            {
                Snackbar.Add("Houve um erro ao cadastrar o setor, tente novamente mais tarde", Severity.Error);
            }
                
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
