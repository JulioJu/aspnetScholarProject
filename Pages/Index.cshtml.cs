using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesContacts.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

// From https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli

namespace RazorPagesContacts.Pages
{
  public class Index : PageModel
  {
    private readonly AppDbContext _db;

    public Index(AppDbContext db)
    {
      _db = db;
    }

    public IList<Customer> Customers { get; private set; }

    public async Task OnGetAsync()
    {
      Customers = await _db.Customers.AsNoTracking().ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
      var contact = await _db.Customers.FindAsync(id);

      if (contact != null)
      {
        _db.Customers.Remove(contact);
        await _db.SaveChangesAsync();
      }

      return RedirectToPage();
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
