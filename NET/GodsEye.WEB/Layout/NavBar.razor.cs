using GodsEye.WEB.Helpers;
using MudBlazor;
using System;

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
                Key = "cadastros",
                Title = "Cadastros",
                Icon = Icons.Material.Filled.Inventory2,
                Items =
                {
                    new() { Title = "Pessoas", Url = "/cadastro/pessoas" },
                    new() { Title = "Setores", Url = "/cadastro/setores" },
                    new() { Title = "Câmeras", Url = "/cadastro/cameras" }
                }
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

            //new NavMenuGroup
            //{
            //    Key = "relatorios",
            //    Title = "Relatórios",
            //    Icon = Icons.Material.Filled.BarChart,
            //    Items =
            //    {
            //        new() { Title = "Ambientes", Url = "/relatorio/monitoramento-ambientes" },
            //        new() { Title = "Incidentes", Url = "/relatorio/captura-incidentes" },
            //        new() { Title = "Permanência", Url = "/relatorio/controle-permanencia" }
            //    }
            //},

            new NavMenuGroup
            {
                Key = "configuracoes",
                Title = "Configurações",
                Icon = Icons.Material.Filled.Settings,
                Items =
                {
                    new() { Title = "Notificações", Url = "/configuracao/notificacao" }
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
