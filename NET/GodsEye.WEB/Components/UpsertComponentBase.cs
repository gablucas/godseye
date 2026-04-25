using GodsEye.WEB.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GodsEye.WEB.Components
{
    public abstract class UpsertComponentBase : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        #region FORM
        protected MudForm form;
        protected bool success;
        protected string[] errors = { };
        protected bool visible = false;
        #endregion
    }

    public abstract class UpsertComponentBase<TService, TModel> : UpsertComponentBase
        where TService : class, IWebService<TModel>
        where TModel : class
    {
        [Inject]
        protected TService Service { get; set; }
    }
}
