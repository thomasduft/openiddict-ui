using Microsoft.AspNetCore.Mvc.Rendering;

namespace Server.Pages.Account.Manage;

public static class ManageNavPages
{
  public static string Index
  {
    get
    {
      return "Index";
    }
  }

  public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

  private static string PageNavClass(ViewContext viewContext, string page)
  {
    var activePage = viewContext.ViewData["ActivePage"] as string
        ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

    return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
  }
}
