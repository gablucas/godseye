using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Components.CameraComponents;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;


namespace GodsEye.WEB.Pages.Camera
{
    public partial class CameraPage
    {
        #region DI

        [Inject]
        public CameraWebService cameraService { get; set; }

        [Inject]
        public SectorWebService SectorService { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }


        #endregion

        #region TABLE PARAMETERS

        List<CameraModel> _cameras = new();
        IEnumerable<CameraModel> _filteredCameras = Enumerable.Empty<CameraModel>();

        private int selectedRowNumber = -1;
        private MudTable<CameraModel> mudTable;
        bool _loading;

        #endregion

        #region TABLE FILTERS

        private string _cameraNameFilter = "";
        private string _conectionNameFilter = "";
        private string _featuresNameFilter = "";

        private List<SectorModel> _sectors = new();
        private IEnumerable<string> _selectedSectors { get; set; } = new HashSet<string>() { };

        #endregion

        bool _visible = true;

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var camerasResult = await cameraService.GetAllAsync();

            if (camerasResult is not null && camerasResult.Success)
                _cameras = camerasResult.Data.ToList();
                _filteredCameras = _cameras.ToList();

            var sectorsRequest = await SectorService.GetAllAsync();
            if (sectorsRequest.Success)
                _sectors = sectorsRequest.Data.ToList();

            _loading = false;
        }

        #region TABLE FUNCTIONS
        private void RowClickEvent(TableRowClickEventArgs<CameraModel> tableRowClickEventArgs)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False };
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

        #region DIALOG FUNCS

        private async Task OpenCreateCamera()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False };
            var dialog = await DialogService.ShowAsync<CreateCameraComponent>("Criar camera", options);

            var result = await dialog.Result;

            if (result.Canceled)
                return;

            if (result.Data is not int cameraId || cameraId <= 0)
            {
                Snackbar.Add("ID inválido retornado", Severity.Error);
                return;
            }

            var newCamera = await cameraService.GetById(cameraId);

            if (newCamera is null || !newCamera.Success)
                return;

            _cameras.Insert(0, newCamera.Data);
        }

        private async Task OpenEditData(int cameraId)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False };
            var parameters = new DialogParameters<CameraDataComponent> { { x => x.Id, cameraId } };
            var dialog = await DialogService.ShowAsync<CameraDataComponent>("Editar dados camera", parameters, options);

            var result = await dialog.Result;
        }

        #endregion

        private List<BreadcrumbItem> _items =
        [
            new("Home", href: "/"),
            new("Cadastro", href: null, disabled: true),
            new("Cameras", href: null, disabled: true)
        ];

        private void OnSectorsChanged(IEnumerable<string> values)
        {
            _selectedSectors = values.ToHashSet();
            ApplyFilters();
        }

        private string GetMultiSelectionText(List<string> selectedValues)
        {
            return $"{selectedValues.Count} setor{(selectedValues.Count > 1 ? "es foram selecionados" : " foi selecionado")}";
        }

        void ApplyFilters()
        {
            _filteredCameras = _cameras
                .Where(x =>
                    (string.IsNullOrWhiteSpace(_cameraNameFilter) || x.Name.Contains(_cameraNameFilter, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(_conectionNameFilter) || (x.Connection ?? "").Contains(_conectionNameFilter, StringComparison.OrdinalIgnoreCase) &&
                    (_selectedSectors.Count() == 0 || _selectedSectors.Contains(x.SectorId.ToString()))) &&
                    (string.IsNullOrWhiteSpace(_featuresNameFilter) || x.Features.Any(y => y.FeatureName.Contains(_featuresNameFilter, StringComparison.OrdinalIgnoreCase)))
                ).ToList();
        }
    }
}
