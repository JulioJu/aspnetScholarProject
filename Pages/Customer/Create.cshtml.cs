// Inspired from https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli

namespace Videotheque.Pages.CustomerPage
{
  using System.Globalization;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Videotheque.Data;

  public class Create : PageModel
  {
    private readonly AppDbContext _db;

    public Create(AppDbContext db)
    {
      this._db = db;
    }

    [TempData]
    public string Message { get; set; }

    [BindProperty]
    public Customer Customer { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!base.ModelState.IsValid)
      {
        return base.Page();
      }

      this._db.Customers.Add(this.Customer);
      await this._db.SaveChangesAsync().ConfigureAwait(false);
      this.Message = $"Customer {this.Customer.Lastname} added";
      return base.RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostJoinListAsync()
    {
      return await this.OnPostAsync().ConfigureAwait(false);
    }

    public async Task<IActionResult> OnPostJoinListUCAsync()
    {
      if (!base.ModelState.IsValid)
      {
        return base.Page();
      }
      this.Customer.Lastname = this.Customer.Lastname?
        .ToUpper(CultureInfo.CurrentCulture);
      return await this.OnPostJoinListAsync()
        .ConfigureAwait(false);
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
