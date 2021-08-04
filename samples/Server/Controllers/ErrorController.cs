using Microsoft.AspNetCore.Mvc;

namespace Server
{
  [ApiExplorerSettings(GroupName = "sample-server")]
  public class ErrorController : Controller
  {
    // TODO: enable Error page again!

    // [HttpGet, HttpPost, Route("~/error")]
    // public IActionResult Error()
    // {
    //   // If the error was not caused by an invalid
    //   // OIDC request, display a generic error page.
    //   var response = HttpContext.GetOpenIddictServerResponse();
    //   if (response is null)
    //   {
    //     return View(new ErrorViewModel());
    //   }

    //   return View(new ErrorViewModel
    //   {
    //     Error = response.Error,
    //     ErrorDescription = response.ErrorDescription
    //   });
    // }
  }
}