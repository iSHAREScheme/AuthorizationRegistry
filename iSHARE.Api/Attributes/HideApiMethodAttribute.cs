using iSHARE.Api.Configurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace iSHARE.Api.Attributes
{
    public class HideLiveApiMethodAttribute : HideApiMethodAttribute
    {
        private readonly IHostingEnvironment _environment;

        public HideLiveApiMethodAttribute(IHostingEnvironment environment) : base(environment)
        {
            _environment = environment;
        }

        protected override bool ShouldHide => _environment.IsLiveOrQaLive();
    }

    public class HideProductionMethodAttribute : HideApiMethodAttribute
    {
        private readonly IHostingEnvironment _environment;

        public HideProductionMethodAttribute(IHostingEnvironment environment) : base(environment)
        {
            _environment = environment;
        }

        protected override bool ShouldHide => _environment.IsLiveOrQaLive() || _environment.IsTest();
    }

    public abstract class HideApiMethodAttribute : ActionFilterAttribute
    {
        private readonly IHostingEnvironment _environment;

        protected HideApiMethodAttribute(IHostingEnvironment environment)
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
