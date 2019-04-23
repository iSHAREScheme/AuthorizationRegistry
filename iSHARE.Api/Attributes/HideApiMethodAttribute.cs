using iSHARE.Api.Configurations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace iSHARE.Api.Attributes
{
    public class HideApiMethodAttribute : ActionFilterAttribute
    {
        private readonly IHostingEnvironment _environment;

        public HideApiMethodAttribute(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_environment.IsLive())
            {
                context.Result = new NotFoundResult();
            }

            base.OnActionExecuting(context);
        }
    }
}
