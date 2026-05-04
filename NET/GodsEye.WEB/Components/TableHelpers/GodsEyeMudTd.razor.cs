using Microsoft.AspNetCore.Components;

namespace GodsEye.WEB.Components.TableHelpers
{
    public partial class GodsEyeMudTd<T> where T : notnull
    {

        [Parameter]
        public string DataLabel { get; set; } = String.Empty;

        [Parameter]
        public string Label { get; set; } = String.Empty;

        [Parameter]
        public T Value { get; set; }

        [Parameter]
        public EventCallback ApplyFilterCallback { get; set; }

        #region Text Filter

        [Parameter]
        public T FilterText { get; set; }

        [Parameter]
        public EventCallback<T> FilterTextChanged { get; set; }

        [Parameter]
        public IGodsEyeMudTdTypeEnum Type { get; set; }

        #endregion

        #region List Filter

        [Parameter]
        public HashSet<T> FilterList { get; set; } = new();

        [Parameter]
        public EventCallback<HashSet<T>> FilterListChanged { get; set; }

        #endregion

        private async Task OnFilterOptionChanged(T? value)
        {
            switch (Type)
            {
                case IGodsEyeMudTdTypeEnum.TEXT:
                    FilterText = value ?? default!;
                    await FilterTextChanged.InvokeAsync(FilterText);
                    break;

                case IGodsEyeMudTdTypeEnum.HASHSET:
                    if (FilterList.Contains(Value))
                        FilterList.Remove(Value);
                    else
                        FilterList.Add(Value);

                    await FilterListChanged.InvokeAsync(FilterList);
                    break;
            }

            await ApplyFilterCallback.InvokeAsync();
        }

    }
}
