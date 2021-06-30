using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tomware.OpenIddict.UI.Identity.Api
{
  internal static class RoutePrefixExtension
  {
    internal static void UseOpenIddictUIIdentityRoutePrefix(this MvcOptions opts, string prefix)
    {
      opts.Conventions.Insert(0, new RoutePrefixConvention(new RouteAttribute(prefix)));
    }
  }

  internal class RoutePrefixConvention : IApplicationModelConvention
  {
    private readonly AttributeRouteModel _routePrefix;

    public RoutePrefixConvention(IRouteTemplateProvider route)
    {
      _routePrefix = new AttributeRouteModel(route);
    }

    public void Apply(ApplicationModel application)
    {
      var controllerTypes = new List<Type>
      {
        typeof(AccountController),
        typeof(ClaimTypeController),
        typeof(RoleController)
      };

      var controllers = application.Controllers
        .Where(c => controllerTypes.Contains(c.ControllerType));
      foreach (var controller in controllers)
      {
        var matchedSelectors = controller.Selectors
          .Where(x => x.AttributeRouteModel != null)
          .ToList();
        if (matchedSelectors.Any())
        {
          foreach (var selectorModel in matchedSelectors)
          {
            selectorModel.AttributeRouteModel = AttributeRouteModel
              .CombineAttributeRouteModel(_routePrefix, selectorModel.AttributeRouteModel);
          }
        }

        var unmatchedSelectors = controller.Selectors
          .Where(x => x.AttributeRouteModel == null)
          .ToList();
        if (unmatchedSelectors.Any())
        {
          foreach (var selectorModel in unmatchedSelectors)
          {
            selectorModel.AttributeRouteModel = _routePrefix;
          }
        }
      }
    }
  }
}