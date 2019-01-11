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
    }

    public override async Task<IActionResult> OnGetAsync(int? id,
        bool? saveChangeErrors = false)
    {
      string currentRoute = base.HttpContext.Request.Path;
      if (currentRoute.Contains("/Edit/",
            System.StringComparison.InvariantCultureIgnoreCase))
      {
        this.IsInvoice = false;
        this.ArticleIdToBorrowArrayInputValue =
          new string[this.numberInputArticleToBorrow];
        this.ValidationMessageArticleIdToBorrowArray =
          new string[this.numberInputArticleToBorrow];
      }
      return await base.OnGetAsync(id, saveChangeErrors).ConfigureAwait(false);
    }

    private protected async override Task<Customer>
      PerformSearchInDatabaseFunc(int? id)
    {
      return await base._tDbSet
        .Include(s => s.CurrentlyBorrowed)
          .ThenInclude(f => f.Film)
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Id == id)
        .ConfigureAwait(false);
    }

  }

}

// vim:sw=2:ts=2:et:fileformat=dos
