namespace Videotheque.Pages.CustomerPage
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed partial class Crud : CrudAbstract<Customer>
  {
    public Crud(AppDbContext db)
      : base(db, db.Customers)
    {
      this.ValidationMessageArticleIdToBorrowArray = new string[4];
      this.ArticleIdToBorrowArrayInputValue = new string[4];
    }

    private protected async override Task<Customer>
      PerformSearchInDatabaseFunc(int? id)
    {
      return await base._tDbSet
        .Include(s => s.CurrentlyBorrowed)
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Id == id)
        .ConfigureAwait(false);
    }

    public override async Task<IActionResult> OnGetAsync(int? id,
        bool? saveChangeErrors = false)
    {
      return await base.OnGetAsyncWithFunc(id, this.PerformSearchInDatabaseFunc)
        .ConfigureAwait(false);
    }

  }

}

// vim:sw=2:ts=2:et:fileformat=dos
