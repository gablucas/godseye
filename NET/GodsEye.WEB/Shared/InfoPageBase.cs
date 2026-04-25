using GodsEye.WEB.Services;
using GodsEye.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace GodsEye.WEB.Shared
{
    public abstract class InfoPageBase<TService, TModel> : ComponentBase
        where TService : class, IWebService<TModel>
        where TModel : class
    {
        [Inject]
        protected TService Service { get; set; }

        [Inject]
        protected NewDialogWebService DialogWebService { get; set; }

        protected bool _loading;

        protected IEnumerable<TModel> _items = new List<TModel>();
        protected IEnumerable<TModel> _filteredItems = Enumerable.Empty<TModel>();

        protected override async Task OnInitializedAsync()
        {
            _loading = true;

            await OnBeforeLoad();

            var result = await LoadDataAsync();

            if (result is not null)
            {
                _items = result.ToList();
                _filteredItems = _items;
            }

            await OnAfterLoad();

            _loading = false;
        }

        protected virtual async Task<IEnumerable<TModel>?> LoadDataAsync()
        {
            return await Service.GetAllAsync();
        }

        protected virtual Task OnBeforeLoad()
            => Task.CompletedTask;

        protected virtual Task OnAfterLoad()
            => Task.CompletedTask;

        protected virtual void ApplyFilters() 
        {
            _filteredItems = _items;
        }
    }
}
