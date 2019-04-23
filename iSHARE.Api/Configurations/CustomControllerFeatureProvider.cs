using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace iSHARE.Api.Configurations
{
    public class CustomControllerFeatureProvider : ControllerFeatureProvider
    {
        private readonly Type[] _excludedControllers = new Type[] { };

        public CustomControllerFeatureProvider(Type[] excludedControllers)
        {
            _excludedControllers = excludedControllers;
        }
        protected override bool IsController(TypeInfo typeInfo)
        {
            var isController = base.IsController(typeInfo);
            if (!isController)
            {
                return false;
            }

            if (_excludedControllers.Any() && _excludedControllers.Contains(typeInfo))
            {
                return false;
            }

            return true;
        }
    }

    public static class CustomControllerFeatureProviderBuilder
    {
        public static IMvcCoreBuilder ExcludeControllers(this IMvcCoreBuilder mvcBuilder, params Type[] excludedControllers)
        {
            mvcBuilder.ConfigureApplicationPartManager(manager =>
            {
                var controllerFeatureProvider =
                    manager.FeatureProviders
                        .Single(p => p.GetType() == typeof(ControllerFeatureProvider));
                manager.FeatureProviders[manager.FeatureProviders.IndexOf(controllerFeatureProvider)] = new CustomControllerFeatureProvider(excludedControllers);
            });

            return mvcBuilder;
        }
    }
}
