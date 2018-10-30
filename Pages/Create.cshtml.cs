using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesContacts.Data;

// Inspired from https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli

namespace RazorPagesContacts.Pages
{
  public class CreateModel : PageModel
  {
    private readonly AppDbContext _db;

    public CreateModel(AppDbContext db)
    {
      _db = db;
    }

    [BindProperty]
    public Customer Customer { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }


      System.Console.WriteLine("coucou" + Customer);

      _db.Customers.Add(Customer);
      await _db.SaveChangesAsync();
      return RedirectToPage("/Index");
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
