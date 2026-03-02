using GodsEye.Application.DTOs.Model;
using GodsEye.WEB.Components.AccessSchedule;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Pages.AccessCalendar
{
    public partial class AccessCalendarPage
    {
        #region DI

        [Inject]
        AccessScheduleWebService AccessScheduleWebSerice { get; set; }

        [Inject]
        public IDialogService DialogService { get; set; }

        #endregion

        
        private List<BreadcrumbItem> _items =
        [
            new("Home", href: "/"),
            new("Cadastro", href: null, disabled: true),
            new("Calendário de acesso", href: null, disabled: true)
        ];

        #region TABLE PARAMETERS

        private MudTable<AccessScheduleModel> mudTable;
        List<AccessScheduleModel> _accessSchedule = new();
        List<AccessScheduleModel> _filteredAccessSchedule = new();

        

        private int selectedRowNumber = -1;
        
        bool _loading;

        #endregion

        #region TABLE FILTERS

        private string _accessScheduleFilter = "";

        #endregion

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            var accessScheduleResult = await AccessScheduleWebSerice.GetAllAsync();

            if (accessScheduleResult is not null && accessScheduleResult.Success && accessScheduleResult.Data is not null)
            {
                _accessSchedule = accessScheduleResult.Data.ToList();
                _filteredAccessSchedule = _accessSchedule.ToList();

            }

            _loading = false;
        }

        #region DIALOG FUNCS

        private async Task OpenCreateCamera()
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False };
            var dialog = await DialogService.ShowAsync<CreateAccessScheduleComponent>("Criar calendário de acesso", options);

            var result = await dialog.Result;

            if (result.Canceled)
                return;

            if (result.Data is not int cameraId || cameraId <= 0)
            {
                Snackbar.Add("ID inválido retornado", Severity.Error);
                return;
            }

            var newAccessSchedule = await AccessScheduleWebSerice.GetById(cameraId);

            if (newAccessSchedule is null || !newAccessSchedule.Success)
                return;

            _accessSchedule.Insert(0, newAccessSchedule.Data);
            ApplyFilters();
        }

        private async Task OpenEditData(int accessSchedule)
        {
            var options = new DialogOptions { CloseOnEscapeKey = true, FullWidth = true, MaxWidth = MaxWidth.False, NoHeader = true };
            var parameters = new DialogParameters<CreateAccessScheduleComponent> { { x => x.Id, accessSchedule } };
            var dialog = await DialogService.ShowAsync<CreateAccessScheduleComponent>("Atualizar calendário de acesso", parameters, options);

            var result = await dialog.Result;
        }

        #endregion

        void ApplyFilters()
        {
            _filteredAccessSchedule = _accessSchedule
                .Where(x =>
                    (string.IsNullOrWhiteSpace(_accessScheduleFilter) || x.Name.Contains(_accessScheduleFilter, StringComparison.OrdinalIgnoreCase))
                ).ToList();
        }
    }
}
