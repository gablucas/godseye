using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Components.SectorComponents;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Pages.Sector
{
    public partial class SectorPage
    {
        #region DI

        [Inject]
        public SectorWebService sectorService { get; set; }

        [Inject]
        public CameraWebService cameraService { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        #endregion

        #region TABLE PARAMETERS

        IEnumerable<SectorModel> _sectors = Enumerable.Empty<SectorModel>();
        IEnumerable<SectorModel> _filteredSectors = Enumerable.Empty<SectorModel>();
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
                _sectors = sectorsResult.Data;
                _filteredSectors = _sectors;
            }

            var camerasRequest = await cameraService.GetAllAsync();
            if (camerasRequest.Success)
                _camerasFilter = camerasRequest.Data.ToList();


            _loading = false;
        }

        private void OpenCreateSector()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.Large };
            DialogService.ShowAsync<CreateSectorComponent>("Criar setor", options);
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
