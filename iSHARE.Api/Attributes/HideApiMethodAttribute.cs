using iSHARE.Api.Configurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace iSHARE.Api.Attributes
{
    public class HideLiveApiMethodAttribute : HideApiMethodAttribute
    {
        private readonly IWebHostEnvironment _environment;

        public HideLiveApiMethodAttribute(IWebHostEnvironment environment) : base(environment)
        {
            _environment = environment;
        }

        protected override bool ShouldHide => _environment.IsLiveOrQaLive();
    }

    public class HideProductionMethodAttribute : HideApiMethodAttribute
    {
        private readonly IWebHostEnvironment _environment;

        public HideProductionMethodAttribute(IWebHostEnvironment environment) : base(environment)
        {
            _environment = environment;
        }

        protected override bool ShouldHide => _environment.IsLiveOrQaLive() || _environment.IsTest();
    }

    public abstract class HideApiMethodAttribute : ActionFilterAttribute
    {
        private readonly IWebHostEnvironment _environment;

        protected HideApiMethodAttribute(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (ShouldHide)
            {
                context.Result = new NotFoundResult();
            }

            base.OnActionExecuting(context);
        }

        protected abstract bool ShouldHide { get; }
    }
}
