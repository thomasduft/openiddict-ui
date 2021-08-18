using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace tomware.OpenIddict.UI.Suite.Api
{
  public static class RoutePrefixExtension
  {
    public static void UseOpenIddictUIRoutePrefix(
      this MvcOptions opts,
      string prefix,
      IEnumerable<Type> controllerTypes
    )
    {
      opts.Conventions.Insert(0, new RoutePrefixConvention(
        new RouteAttribute(prefix),
        controllerTypes
      ));
    }
  }

  internal class RoutePrefixConvention : IApplicationModelConvention
  {
    private readonly AttributeRouteModel _routePrefix;
    private readonly IEnumerable<Type> _controllerTypes;

    public RoutePrefixConvention(IRouteTemplateProvider route, IEnumerable<Type> controllerTypes)
    {
      _routePrefix = new AttributeRouteModel(route);
      _controllerTypes = controllerTypes 
        ?? throw new ArgumentNullException(nameof(controllerTypes));
    }

    public void Apply(ApplicationModel application)
    {
      var controllers = application.Controllers
        .Where(c => _controllerTypes.Contains(c.ControllerType));
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