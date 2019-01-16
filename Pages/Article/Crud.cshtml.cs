namespace Videotheque.Pages.ArticlePage
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed partial class Crud : CrudAbstract<Article>
  {
    public Crud(AppDbContext db)
      : base(db, db.Articles)
    {
    }

    // Used under /Page/Customer/Crud.CreateOrEdit.shtml.cs
    public static async Task<Article>
      FindArticleAsync(AppDbContext context, int id)
    {
      return await context.Articles
        .Include(a => a.Borrower)
        .Include(a => a.Film)
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Id == id);
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
          s => s.BorrowerId);
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
      return await base.OnPostCreateAsyncWithFunc(
          this.PerformTestOverpostingFunc);
    }

    public async Task<IActionResult> OnPostEditAsync()
    {
      return await base
        .OnPostEditAsyncWithFunc(this.PerformTestOverpostingFunc);
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
