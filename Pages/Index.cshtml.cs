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
    private readonly AppDbContext _db;

    public Index(AppDbContext db)
    {
      this._db = db;
    }

    public IList<Customer> Customers { get; private set; }

    public async Task OnGetAsync()
    {
      this.Customers = await this._db.Customers.AsNoTracking().ToListAsync().
        ConfigureAwait(false);
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
      var contact = await this._db.Customers.FindAsync(id)
        .ConfigureAwait(false);

      if (contact != null)
      {
        this._db.Customers.Remove(contact);
        await this._db.SaveChangesAsync().ConfigureAwait(false);
      }

      return base.RedirectToPage();
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
