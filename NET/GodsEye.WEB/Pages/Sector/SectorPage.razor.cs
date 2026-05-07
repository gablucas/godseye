
using GodsEye.Shared.Response.Camera;
using GodsEye.Shared.Response.Sector;
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
        public DialogWebService DialogWebService { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        #endregion

        #region TABLE PARAMETERS

        List<SectorResponse> _sectors = new();
        List<SectorResponse> _filteredSectors = new();
        bool _loading;

        #endregion

        #region TABLE FILTERS

        private string _sectorNameFilter = "";

        private string _personNameFilter = "";

        private List<CameraResponse> _camerasFilter = new();
        private IEnumerable<string> _selectedDevices { get; set; } = new HashSet<string>() { };

        private string _personFilter = "";

        #endregion

        private List<BreadcrumbItem> _items =
        [
            new("Home", href: "/"),
            new("Organização", href: null, disabled: true),
            new("Setores", href: null, disabled: true)
        ];


        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var sectorsResult = await sectorService.GetAllAsync();

            if (sectorsResult is not null)
            {
                var sector = sectorsResult.ToList();
                var lookupSectors = sector.ToLookup(x => x.ParentId);

                _sectors = BuildTree(null, lookupSectors);
            }

            var camerasRequest = await cameraService.GetAllAsync();
            if (camerasRequest is not null)
                _camerasFilter = camerasRequest.ToList();


            _loading = false;
        }

        private List<SectorResponse> BuildTree(int? parentId, ILookup<int?, SectorResponse> lookup)
        {
            return lookup[parentId]
                .Select(x => new SectorResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    ParentId = x.ParentId,
                    Children = BuildTree(x.Id, lookup) // 🔁 recursão
                })
                .ToList();
        }

        private async Task CreateSectorCallback(int sectorId)
        {
            var newSector = await sectorService.GetById(sectorId);

            if (newSector is null)
                return;

            _sectors.Insert(0, newSector);
            ApplyFilters();
        }

        private void OnCamerasChanged(IEnumerable<string> values)
        {
            _selectedDevices = values.ToHashSet();
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
                    (string.IsNullOrWhiteSpace(_sectorNameFilter) || x.Name.Contains(_sectorNameFilter, StringComparison.OrdinalIgnoreCase)) 
                    //&& (_selectedDevices.Count() == 0 || x.Devices.Any(c => _selectedDevices.ToList().Contains(c.Id.ToString())))
                ).ToList();
        }
    }
}
