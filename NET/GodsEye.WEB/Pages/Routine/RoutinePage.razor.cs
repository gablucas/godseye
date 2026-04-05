using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Pages.Routine
{
    public partial class RoutinePage
    {
        #region DI

        [Inject]
        public SectorWebService sectorService { get; set; }

        [Inject]
        public CameraWebService cameraService { get; set; }

        [Inject]
        public DialogWebService DialogWebService { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        #endregion

        #region TABLE PARAMETERS

        List<SectorModel> _sectors = new();
        List<SectorModel> _filteredSectors = new();
        bool _loading;

        #endregion

        #region TABLE FILTERS

        private string _sectorNameFilter = "";

        private string _personNameFilter = "";

        private List<CameraModel> _camerasFilter = new();
        private IEnumerable<string> _selectedCameras { get; set; } = new HashSet<string>() { };

        private string _personFilter = "";

        #endregion

        private List<BreadcrumbItem> _items =
        [
            new("Home", href: "/"),
            new("Cadastro", href: null, disabled: true),
            new("Setores", href: null, disabled: true)
        ];


        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var sectorsResult = await sectorService.GetAllAsync();

            if (sectorsResult is not null && sectorsResult.Success)
            {
                _sectors = sectorsResult.Data.ToList();
                _filteredSectors = _sectors;
            }

            var camerasRequest = await cameraService.GetAllAsync();
            if (camerasRequest.Success)
                _camerasFilter = camerasRequest.Data.ToList();


            _loading = false;
        }

        private async Task CraeteRoutineCallback(int routineId)
        {
            var newSector = await sectorService.GetById(routineId);

            if (newSector is null || !newSector.Success)
                return;

            _sectors.Insert(0, newSector.Data);
            ApplyFilters();
        }

        private void OnCamerasChanged(IEnumerable<string> values)
        {
            _selectedCameras = values.ToHashSet();
            ApplyFilters();
        }

        private string GetMultiSelectionText(List<string> selectedValues)
        {
            return $"{selectedValues.Count} setor{(selectedValues.Count > 1 ? "es foram selecionados" : " foi selecionado")}";
        }

        void ApplyFilters()
        {
            _filteredSectors = _sectors
                .Where(x =>
                    (string.IsNullOrWhiteSpace(_sectorNameFilter) || x.Name.Contains(_sectorNameFilter, StringComparison.OrdinalIgnoreCase)) &&
                    (_selectedCameras.Count() == 0 || x.Cameras.Any(c => _selectedCameras.ToList().Contains(c.Id.ToString())))
                ).ToList();
        }
    }
}
