using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Components;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;


namespace GodsEye.WEB.Pages.Camera
{
    public partial class CameraListPage
    {
        #region DI

        [Inject]
        public CameraService cameraService { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }


        #endregion


        #region TABLE PARAMETERS

        IEnumerable<CameraModel> _cameras = Enumerable.Empty<CameraModel>();
        private int selectedRowNumber = -1;
        private MudTable<CameraModel> mudTable;
        bool _loading;

        #endregion

        bool _visible = true;

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var camerasResult = await cameraService.GetAllAsync();

            if (camerasResult is not null && camerasResult.Sucesso)
                _cameras = camerasResult.Dados;

            _loading = false;
        }

        #region TABLE FUNCTIONS
        private void RowClickEvent(TableRowClickEventArgs<CameraModel> tableRowClickEventArgs)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.Large };
            var parameters = new DialogParameters<InfoCameraComponent> { { x => x.Camera, tableRowClickEventArgs.Item } };

            DialogService.ShowAsync<InfoCameraComponent>("Simple Dialog", parameters, options);
        }

        private string SelectedRowClassFunc(CameraModel element, int rowNumber)
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
