using GodsEye.WEB.Helpers;
using MudBlazor;

namespace GodsEye.WEB.Layout
{
    public partial class NavBar
    {
        private string _openGroup;

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
                Key = "monitoramentos",
                Title = "Monitoramento",
                Icon = Icons.Material.Filled.Visibility,
                Items =
                {
                    new() { Title = "Ambientes", Url = "/monitoramento/monitoramento-ambientes" },
                    new() { Title = "Incidentes", Url = "/monitoramento/captura-incidentes" },
                    new() { Title = "Permanência", Url = "/monitoramento/controle-permanencia" }
                }
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
                Key = "controle-acesso",
                Title = "Controle de Acesso",
                Icon = Icons.Material.Filled.LockPerson,
                Items =
                {
                    new() { Title = "Níveis de Acesso", Url = "/controle-acesso/nivel" },
                    new() { Title = "Horários de acesso", Url = "/controle-acesso/horarios" }
                }
            }
        };

        #endregion

        protected override void OnInitialized()
        {
            SyncGroupWithRoute();
            Nav.LocationChanged += (_, __) => SyncGroupWithRoute();
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
