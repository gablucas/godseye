using GodsEye.Shared.Enums;
using MudBlazor;

namespace GodsEye.WEB.Pages.Compliance
{
    public partial class ComplianceViolationsPage
    {
        #region COMPLIANCE MENU

        private CompliancePolicyEnum? _selectedMenu = null;

        private void SelectMenuOption(CompliancePolicyEnum? selectedMenu)
        {
            _selectedMenu = selectedMenu;
        }

        private string IsMenuSelected(CompliancePolicyEnum? selectedMenu)
        {
            if (_selectedMenu == selectedMenu)
                return "selected";

            return "";
        }

        #endregion

        private List<BreadcrumbItem> _breadCrumb =
        [
            new("Home", href: "/"),
            new("Compliance", href: null, disabled: true),
            new("Violações", href: null, disabled: true)
        ];
    }
}
