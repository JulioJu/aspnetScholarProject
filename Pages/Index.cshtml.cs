// From https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli

namespace RazorPagesContacts.Pages
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Microsoft.EntityFrameworkCore;
  using RazorPagesContacts.Data;

  public class Index : PageModel
  {
    private readonly AppDbContext db;

    public Index(AppDbContext db)
    {
      this.db = db;
    }

    public IList<Customer> Customers { get; private set; }

    public async Task OnGetAsync()
    {
      this.Customers = await this.db.Customers.AsNoTracking().ToListAsync().
        ConfigureAwait(false);
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
      var contact = await this.db.Customers.FindAsync(id)
        .ConfigureAwait(false);

      if (contact != null)
      {
        this.db.Customers.Remove(contact);
        await this.db.SaveChangesAsync().ConfigureAwait(false);
      }

      return base.RedirectToPage();
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
