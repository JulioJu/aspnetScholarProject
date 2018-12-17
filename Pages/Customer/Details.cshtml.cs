namespace Videotheque.Pages.CustomerPage
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed class Details : DetailsAbstract<Customer>
  {
    public Details(AppDbContext db)
      : base(db, db.Customers)
    {
    }

    public async override Task<Customer> PerformSearchInDatabaseFunc(int? id)
    {
      return base.AbstractEntity = await base._tDbSet
        .Include(s => s.CurrentlyBorrowed)
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Id == id)
        .ConfigureAwait(false);
    }

    public async override Task<IActionResult> OnGetAsync(int? id)
    {
      return await base.OnGetAsyncWithFunc(id, this.PerformSearchInDatabaseFunc)
        .ConfigureAwait(false);
    }

  }

}

// vim:sw=2:ts=2:et:fileformat=dos
