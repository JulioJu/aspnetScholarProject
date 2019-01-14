namespace Videotheque.Pages.FilmPage
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed partial class Crud : CrudAbstract<Film>
  {
    public Crud(AppDbContext db)
      : base(db, db.Films)
    {
    }

    [BindProperty]
    public int NumberOfNewArticles { get; set; }

    private protected async override Task<Film>
      PerformSearchInDatabaseFunc(int? id)
    {
      return await base._tDbSet
        .Include(s => s.Articles)
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Id == id)
        ;
    }

    public override async Task<IActionResult> OnGetAsync(int? id,
        bool? saveChangeErrors = false)
    {
      string currentRoute = base.HttpContext.Request.Path;
      if (currentRoute.EndsWith("/Create",
            System.StringComparison.InvariantCultureIgnoreCase))
      {
        this.NumberOfNewArticles = 1;
      }
      else if (currentRoute.Contains("/Edit/",
            System.StringComparison.InvariantCultureIgnoreCase))
      {
        this.NumberOfNewArticles = 0;
      }
      return await base.OnGetAsyncWithFunc(id, this.PerformSearchInDatabaseFunc)
        ;
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
        ;
    }

    private void CreateNewArticles()
    {
      for (int index = 0; index < this.NumberOfNewArticles; index++)
      {
        Article article = new Article();
        article.Film = base.AbstractEntity;
        base._db.Articles.Add(article);
      }
      this._tDbSet.Add(this.AbstractEntity);
    }

    public override async Task<IActionResult> OnPostCreateAsync()
    {
      this.CreateNewArticles();
      return await base.OnPostCreateAsyncWithFunc(
          this.PerformTestOverpostingFunc)
        ;
    }

    public async Task<IActionResult> OnPostEditAsync()
    {
      this.CreateNewArticles();
      return await base.OnPostEditAsyncWithFunc(this.PerformTestOverpostingFunc)
        ;
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
