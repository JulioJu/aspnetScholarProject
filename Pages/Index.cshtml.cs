// From https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli

namespace RazorPagesIntro.Pages
{
  using System;
  using Microsoft.AspNetCore.Mvc.RazorPages;

  public class Index : PageModel
  {
    public string Message { get; private set; } = "PageModel in C#";

    public void OnGet()
    {
      Message += $" Server time is {DateTime.Now}";
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
