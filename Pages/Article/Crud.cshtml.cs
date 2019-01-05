namespace Videotheque.Pages.ArticlePage
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed partial class Crud : CrudAbstract<Article>
  {
    public Crud(AppDbContext db)
      : base(db, db.Articles)
    {
    }

    private protected async override Task<Article>
      PerformSearchInDatabaseFunc(int? id)
    {
      return await base._tDbSet
        .Include(a => a.Borrower)
        .Include(a => a.Film)
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Id == id)
        .ConfigureAwait(false);
    }

    public override async Task<IActionResult> OnGetAsync(int? id,
        bool? saveChangeErrors = false)
    {
      base.ViewData["BorrowerId"] = new SelectList(base._db.Customers,
          "Id",
          "Address");
      base.ViewData["FilmId"] = new SelectList(base._db.Films,
          "Id",
          "Title");
      return await base.OnGetAsyncWithFunc(id, this.PerformSearchInDatabaseFunc)
        .ConfigureAwait(false);
    }

    private protected override async Task<bool> PerformTestOverpostingFunc()
    {
      return await base.TryUpdateModelAsync<Article>(
          base.AbstractEntity,
          "article",   // Prefix for form value.
          s => s.Disc,
          s => s.Box,
          s => s.IsLost,
          s => s.CountBorrowing,
          s => s.Comment,
          s => s.BorrowingDate,
          s => s.ReturnDate,
          s => s.FilmId,
          s => s.BorrowerId)
        .ConfigureAwait(false);
    }

    public override async Task<IActionResult> OnPostCreateAsync()
    {
      return await base.OnPostCreateAsyncWithFunc(
          this.PerformTestOverpostingFunc)
        .ConfigureAwait(false);
    }

    public async Task<IActionResult> OnPostEditAsync()
    {
      return await base.OnPostEditAsyncWithFunc(this.PerformTestOverpostingFunc)
        .ConfigureAwait(false);
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
