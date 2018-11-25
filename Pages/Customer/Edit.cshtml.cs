namespace Videotheque.Pages.CustomerPage
{
  using System;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;

  public class Edit : PageModel
  {
    private readonly AppDbContext _db;

    public Edit(AppDbContext db)
    {
      this._db = db;
    }

    [BindProperty]
    public Customer Customer { get; set; }

    [ViewData]
    public string Title { get; } = "Edit Customer —";

    public async Task<IActionResult> OnGetAsync(int id)
    {
      Console.Out.WriteLine(id);
      this.Customer = await this._db.Customers.FindAsync(id)
        .ConfigureAwait(false);

      if (this.Customer == null)
      {
        return base.RedirectToPage("/Index");
      }

      return base.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!base.ModelState.IsValid)
      {
        return base.Page();
      }

      this._db.Attach(this.Customer).State = EntityState.Modified;

      try
      {
        await this._db.SaveChangesAsync().ConfigureAwait(false);
      }
      catch (DbUpdateConcurrencyException)
      {
        #pragma warning disable S112
        throw new Exception($"Customer {this.Customer.Id} not found!");
      }

      return base.RedirectToPage("/Index");
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
