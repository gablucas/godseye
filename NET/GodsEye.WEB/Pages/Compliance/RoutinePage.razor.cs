using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Pages.Compliance
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

        [Inject]
        public SignalRService SignalR { get; set; }

        #endregion

        #region TABLE PARAMETERS

        private MudTable<RoutineModel> mudTable;
        List<RoutineModel> _routines = new();
        List<RoutineModel> _filteredRoutines = new();
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
            new("Permanência", href: null, disabled: true),
            new("Rotinas", href: null, disabled: true)
        ];


        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var routinesResult = await RoutineService.GetAllAsync();

            if (routinesResult is not null)
            {
                _routines = routinesResult.ToList();
                _filteredRoutines = _routines;
            }

            SignalR.Create("https://localhost:7010/createdDataHub");

            SignalR.On<RoutineModel>(
                "CreatedRoutine",
                routine =>
                {
                    _routines = _routines.Where(x => x.Id != routine.Id).ToList();
                    _routines.Insert(0, routine);
                    ApplyFilters();

                    InvokeAsync(() =>
                    {
                        mudTable?.ReloadServerData();
                        StateHasChanged();
                    });
                });

            await SignalR.StartAsync();


            _loading = false;
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
