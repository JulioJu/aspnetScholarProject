namespace Videotheque.Pages
{
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;

  public class EntityIndexRedirection : PageModel
  {
    public IActionResult OnGet()
    {
      return base.RedirectToPage("ShowAll");
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
