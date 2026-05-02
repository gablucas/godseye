using Microsoft.AspNetCore.Components;

namespace GodsEye.WEB.Components.FilterComponents
{
    public partial class InputFilterComponent
    {

        [Parameter]
        public string TypedValue { get; set; }

        [Parameter]
        public EventCallback<string> TypedValueChanged { get; set; }

        [Parameter]
        public EventCallback ApplyFilterCallback { get; set; }

        [Parameter]
        public string Label { get; set; }

        private async Task OnValueChanged(string value)
        {
            TypedValue = value;
            await TypedValueChanged.InvokeAsync(TypedValue); // notifica o pai
            await ApplyFilterCallback.InvokeAsync();
        }
    }
}
