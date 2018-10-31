// Inspired from https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli

namespace RazorPagesContacts.Pages
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using RazorPagesContacts.Data;

  public class Create : PageModel
  {
    private readonly AppDbContext db;

    public Create(AppDbContext db)
    {
      this.db = db;
    }

    [BindProperty]
    public Customer Customer { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!base.ModelState.IsValid)
      {
        return base.Page();
      }

      System.Console.WriteLine("coucou" + this.Customer);

      this.db.Customers.Add(this.Customer);
      await this.db.SaveChangesAsync().ConfigureAwait(false);
      return base.RedirectToPage("/Index");
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
