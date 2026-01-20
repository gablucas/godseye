using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;

namespace GodsEye.WEB.Pages.Sector
{
    public partial class SectorListPage
    {
        [Inject]
        public SectorService sectorService { get; set; }

        IEnumerable<SectorModel> _sectors = Enumerable.Empty<SectorModel>();
        bool _loading;


        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var sectorsResult = await sectorService.GetAllAsync();

            if (sectorsResult is not null && sectorsResult.Success)
                _sectors = sectorsResult.Data;

            _loading = false;
        }
    }
}
