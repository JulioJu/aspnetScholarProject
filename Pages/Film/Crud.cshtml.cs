namespace Videotheque.Pages.FilmPage
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed partial class Crud : CrudAbstract<Film>
  {
    public Crud(AppDbContext db)
      : base(db, db.Films)
    {
    }

    public override async Task<IActionResult> OnGetAsync(int? id,
        bool? saveChangeErrors = false)
    {
      return await base.OnGetAsync(id, saveChangeErrors).ConfigureAwait(false);
    }

    private protected override async Task<bool> PerformTestOverpostingFunc()
    {
      return await base.TryUpdateModelAsync<Film>(
          base.AbstractEntity,
          "film",   // Prefix for form value.
          s => s.Title,
          s => s.Price,
          s => s.ReleaseDate,
          s => s.Image)
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
