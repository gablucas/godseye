using GodsEye.Shared.Response;
using Microsoft.AspNetCore.Components;

namespace GodsEye.WEB.Components.FilterComponents
{
    public partial class SelectFilterComponent<T> where T : IBaseResponse
    {
        [Parameter]
        public List<T> items { get; set; }

        [Parameter]
        public HashSet<string> SelectedItems { get; set; }

        [Parameter]
        public EventCallback<HashSet<string>> SelectedItemsChanged { get; set; }

        [Parameter]
        public EventCallback ApplyFilterCallback { get; set; }

        [Parameter]
        public string Label { get; set; }


        private async Task OnFilterOptionChanged(IEnumerable<string> values)
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
