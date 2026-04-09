using GodsEye.Application.DTOs.Model;
using GodsEye.Application.UseCases.Routine.Queries;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Pages.Routine
{
    public partial class RoutinePage
    {
        #region DI

        [Inject]
        public RoutineWebService RoutineService { get; set; }

        [Inject]
        public DialogWebService DialogWebService { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        #endregion

        #region TABLE PARAMETERS

        List<GetAllRoutineResponse> _routines = new();
        List<GetAllRoutineResponse> _filteredRoutines = new();
        bool _loading;

        #endregion

        #region TABLE FILTERS

        private string _routineFilterName = "";

        private string _personNameFilter = "";

        private List<CameraModel> _camerasFilter = new();
        private IEnumerable<string> _selectedCameras { get; set; } = new HashSet<string>() { };

        private string _personFilter = "";

        #endregion

        private List<BreadcrumbItem> _items =
        [
            new("Home", href: "/"),
            new("Organização", href: null, disabled: true),
            new("Rotinas", href: null, disabled: true)
        ];


        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var routinesResult = await RoutineService.GetAllAsync();

            if (routinesResult is not null && routinesResult.Success)
            {
                _routines = routinesResult.Data.ToList();
                _filteredRoutines = _routines;
            }


            _loading = false;
        }

        private async Task CraeteRoutineCallback(int routineId)
        {
            //var newSector = await sectorService.GetById(routineId);

            //if (newSector is null || !newSector.Success)
            //    return;

            //_routines.Insert(0, newSector.Data);
            //ApplyFilters();
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
            _filteredRoutines = _routines
                .Where(x =>
                    (string.IsNullOrWhiteSpace(_routineFilterName) || x.Name.Contains(_routineFilterName, StringComparison.OrdinalIgnoreCase))
                ).ToList();
        }
    }
}
