namespace Videotheque.Pages.ArticlePage
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed class CreateOrEdit : CreateOrEditAbstract<Article>
  {
    // IHttpContextAccessor needs to be injectected in Startup.cs
    public CreateOrEdit(AppDbContext db,
        IHttpContextAccessor httpContextAccessor)
      : base(db, db.Articles, httpContextAccessor)
    {
    }

    public override async Task<IActionResult> OnGetAsync(int? id,
        bool? saveChangeErrors = false)
    {
      base.ViewData["BorrowerId"] = new SelectList(base._db.Customers,
          "Id",
          "Address");
      return await base.OnGetAsync(id, saveChangeErrors).ConfigureAwait(false);
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

    public async override Task<IActionResult> OnPostCreateAsync()
    {
      return await base.OnPostCreateAsyncWithFunc(
          this.PerformTestOverpostingFunc)
        .ConfigureAwait(false);
    }

    public async override Task<IActionResult> OnPostEditAsync()
    {
      return await base.OnPostEditAsyncWithFunc(this.PerformTestOverpostingFunc)
        .ConfigureAwait(false);
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
