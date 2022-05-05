using Microsoft.AspNetCore.Mvc.Rendering;

namespace ServerWithCustomKey.Pages.Account.Manage
{
  public static class ManageNavPages
  {
    public static string Index => "Index";

    public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

    private static string PageNavClass(ViewContext viewContext, string page)
    {
      var activePage = viewContext.ViewData["ActivePage"] as string
          ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

      return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
    }
  }
}
