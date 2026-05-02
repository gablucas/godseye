
using GodsEye.Shared.Response;
using GodsEye.WEB.Helpers;
using GodsEye.WEB.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Layout
{
    public partial class NavBar
    {
        [Inject]
        public SignalRService SignalR { get; set; }

        private string _openGroup;
        private int _alertCounter;

        #region LINKS

        private List<NavMenuGroup> _menus = new()
        {
            new NavMenuGroup
            {
                Title = "Dashboard",
                Icon = Icons.Material.Filled.Dashboard,
                Url = "/"
            },

            new NavMenuGroup
            {
                Key = "organizacao",
                Title = "Organização",
                Icon = Icons.Material.Filled.BusinessCenter,
                Items =
                {
                    new() { Title = "Pessoas", Url = "/organizacao/pessoas" },
                    new() { Title = "Setores", Url = "/organizacao/setores" },
                    new() { Title = "Calendário de acesso", Url = "/organizacao/calendario-de-acesso" },
                    new() { Title = "Níveis de Acesso", Url = "/organizacao/nivel-de-acesso" },
                    new() { Title = "Emails", Url = "/organizacao/emails" }

                }
            },

            new NavMenuGroup
            {
                Key = "dispositivos",
                Title = "Dispositivos",
                Icon = Icons.Material.Filled.Devices,
                Items =
                {
                    new() { Title = "Câmeras", Url = "/dispositivos/cameras" },

                }
            },

            new NavMenuGroup
            {
                Key = "monitoramentos",
                Title = "Monitoramento",
                Icon = Icons.Material.Filled.Visibility,
                Items =
                {
                    new() { Title = "Tempo real", Url = "/monitoramento-de-ambientes/tempo-real" },
                }
            },

            new NavMenuGroup
            {
                Key = "captura-incidentes",
                Title = "Incidentes",
                Icon = Icons.Material.Filled.ReportProblem,
                Items =
                {
                    new() { Title = "Tempo real", Url = "/captura-de-incidentes/tempo-real" },
                }
            },

            new NavMenuGroup
            {
                Key = "compliance",
                Title = "Compliance",
                Icon = Icons.Material.Filled.Policy,
                Items =
                {
                    new() { Title = "Violações", Url = "/compliance/violacoes" },
                    new() { Title = "Politicas", Url = "/compliance/politicas" }
                }
            },
        };

        #endregion

        protected override void OnInitialized()
        {
            SyncGroupWithRoute();
            Nav.LocationChanged += (_, __) => SyncGroupWithRoute();
        }

        protected override async Task OnInitializedAsync()
        {

            SignalR.Create("https://localhost:7010/createdDataHub");

            SignalR.On<ViolationAlertFeatureResponse>(
                "AlertNotification",
                quantity =>
                {
                    _alertCounter += 1;

                    InvokeAsync(StateHasChanged);
                });

            await SignalR.StartAsync();

        }

        private void ToggleGroup(string key)
        {
            _openGroup = _openGroup == key ? null : key;
        }

        private bool IsGroupOpen(NavMenuGroup group)
        {
            if (_openGroup == group.Key)
                return true;

            return group.Items.Any(i => SelectedMenu(i.Url));
        }

        private bool SelectedMenu(string url)
        {
            return Nav.Uri.EndsWith(url, StringComparison.OrdinalIgnoreCase);
        }

        private void SyncGroupWithRoute()
        {
            var uri = Nav.Uri.ToLower();

            _openGroup = _menus
                .FirstOrDefault(m => m.HasChildren && m.Items.Any(i => uri.Contains(i.Url)))
                ?.Key;

            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            Nav.LocationChanged -= (_, __) => SyncGroupWithRoute();
        }

    }
}
