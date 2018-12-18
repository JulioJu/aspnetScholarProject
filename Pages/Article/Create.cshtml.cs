namespace Videotheque.Pages.ArticlePage
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed class Create : CreateAbstract<Article>
  {
    // IHttpContextAccessor needs to be injectected in Startup.cs
    public Create(AppDbContext db)
      : base(db, db.Articles)
    {
    }

    private protected override async Task<bool> PerformTestOverpostingFunc()
    {
      return await base.TryUpdateModelAsync<Article>(
          base.AbstractEntity,
          "article",   // Prefix for form value.
          s => s.Barcode,
          s => s.Disc,
          s => s.Box,
          s => s.IsLost,
          s => s.CountBorrowing,
          s => s.Comment,
          s => s.BorrowingDate,
          s => s.ReturnDate)
        .ConfigureAwait(false);
    }

    public async override Task<IActionResult> OnPostAsync()
    {
      return await base.OnPostAsyncWithFunc(this.PerformTestOverpostingFunc)
        .ConfigureAwait(false);
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
