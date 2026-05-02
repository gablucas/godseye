using Microsoft.AspNetCore.Components;

namespace GodsEye.WEB.Components.FilterComponents
{
    public partial class SelectFilterEnumComponent<TEnum> where TEnum : struct, System.Enum
    {
        [Parameter]
        public HashSet<TEnum> SelectedItems { get; set; }

        [Parameter]
        public EventCallback<HashSet<TEnum>> SelectedItemsChanged { get; set; }

        [Parameter]
        public EventCallback ApplyFilterCallback { get; set; }

        [Parameter]
        public string Label { get; set; } = string.Empty;


        private async Task OnFilterOptionChanged(IEnumerable<TEnum> values)
        {
            SelectedItems = values.ToHashSet();
            await SelectedItemsChanged.InvokeAsync(SelectedItems);
            await ApplyFilterCallback.InvokeAsync();
        }


        private string GetMultiSelectionText(List<string> selectedValues)
        {
            return $"{selectedValues.Count} ite{(selectedValues.Count > 1 ? "ns selecionados" : "m selecionado")}";
        }
    }
}
