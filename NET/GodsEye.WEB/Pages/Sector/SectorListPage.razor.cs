using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Components.SectorComponents;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Pages.Sector
{
    public partial class SectorListPage
    {
        #region DI

        [Inject]
        public SectorWebService sectorService { get; set; }

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
        private string _cameraNameFilter = "";

        #endregion

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var sectorsResult = await sectorService.GetAllAsync();

            if (sectorsResult is not null && sectorsResult.Success)
            {
                _sectors = sectorsResult.Data;
                _filteredSectors = _sectors;
            }
                

            _loading = false;
        }

        private void OpenCreateSector()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.Large };
            DialogService.ShowAsync<CreateSectorComponent>("Criar setor", options);
        }

        void ApplyFilters()
        {
            _filteredSectors = _sectors
                .Where(x =>
                    (string.IsNullOrWhiteSpace(_sectorNameFilter) || x.Name.Contains(_sectorNameFilter, StringComparison.OrdinalIgnoreCase)) &&
                    (string.IsNullOrWhiteSpace(_cameraNameFilter) || x.Cameras.Any(y => y.Name.Contains(_sectorNameFilter, StringComparison.OrdinalIgnoreCase)))
                ).ToList();
        }
    }
}
