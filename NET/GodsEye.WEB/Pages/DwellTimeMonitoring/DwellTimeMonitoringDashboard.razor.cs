using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Components;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace GodsEye.WEB.Pages.DwellTimeMonitoring
{
    public partial class DwellTimeMonitoringDashboard
    {

        #region DI

        [Inject]
        public CameraWebService cameraService { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }


        #endregion

        #region TABLE PARAMETERS

        private List<CameraByFeatureModel> _cameras = new();
        private MudTable<CameraByFeatureModel> mudTable;
        private HubConnection? hubConnection;
        bool _loading;
        private int? selectedId = null;
        private int selectedRowNumber = -1;

        #endregion

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var camerasResult = await cameraService.GetByFeatureId(3);

            if (camerasResult is not null)
                _cameras = camerasResult.ToList();

            _loading = false;
        }

        #region TABLE FUNCTIONS
        private void RowClickEvent(TableRowClickEventArgs<CameraByFeatureModel> tableRowClickEventArgs)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.Large };
            var parameters = new DialogParameters<InfoDwellTimeMonitoringComponent> { { x => x.Camera, tableRowClickEventArgs.Item } };

            DialogService.ShowAsync<InfoDwellTimeMonitoringComponent>("Simple Dialog", parameters, options);
        }

        private string SelectedRowClassFunc(CameraByFeatureModel element, int rowNumber)
        {
            if (selectedRowNumber == rowNumber)
            {
                selectedRowNumber = -1;
                return string.Empty;
            }
            else if (mudTable.SelectedItem != null && mudTable.SelectedItem.Equals(element))
            {
                selectedRowNumber = rowNumber;
                return "selected";
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion
    }
}
