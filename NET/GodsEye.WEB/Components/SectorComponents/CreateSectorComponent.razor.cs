
using GodsEye.Shared.Response.NotificationGroups;
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
        NotificationGroupWebService notificationGroupService { get; set; }

        [Inject]
        DialogWebService DialogWebService { get; set; }

        #endregion

        #region FORM

        MudForm form;

        CreateSectorForm CreateSectorForm { get; set; } = new();
        public IEnumerable<string> NotificationGroups { get; set; }

        bool success;
        string[] errors = { };
        bool visible = false;

        #endregion

        IEnumerable<NotificationGroupsResponse> _notificationGroups = Enumerable.Empty<NotificationGroupsResponse>();

        [Parameter]
        public int? ParentId { get; set; }

        [CascadingParameter]
        private IMudDialogInstance MudDialog { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var notificationResponse = await notificationGroupService.GetAllAsync();
            if (notificationResponse is not null)
            {
                _notificationGroups = notificationResponse;
            }
        }

        private string GetSelectNotificationGroupsName(List<string> ids)
        {
            var names = _notificationGroups
                .Where(c => ids.Contains(c.Id.ToString()))
                .Select(c => c.Name);

            return string.Join(", ", names);
        }

        private async Task CreateNewEmailGroupCallback(int newEmailGroupId)
        {
            var newEmailGroup = await notificationGroupService.GetById(newEmailGroupId);

            if (newEmailGroup is not null)
            {
                _notificationGroups = _notificationGroups.Append(newEmailGroup).ToList();

                CreateSectorForm.NotificationGroups = CreateSectorForm.NotificationGroups.Append(newEmailGroupId);
            }
        }

        private async Task Submit()
        {
            visible = true;
            CreateSectorForm.ParentId = ParentId;
            var result = await sectorService.CreateAsync(CreateSectorForm);
            visible = false;

            if (result != 0)
            {
                Snackbar.Add("Setor cadastrado com sucesso!", Severity.Success);
                MudDialog.Close(DialogResult.Ok(result));
            }
            else
            {
                Snackbar.Add("Houve um erro ao cadastrar o setor, tente novamente mais tarde", Severity.Error);
            }
                
        }

        private void Cancel() => MudDialog.Cancel();
    }
}
