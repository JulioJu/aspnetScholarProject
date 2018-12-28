namespace Videotheque.Pages.ArticlePage
{
  using Microsoft.AspNetCore.Http;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed partial class Crud : CrudAbstract<Article>
  {
    public Crud(AppDbContext db, IHttpContextAccessor httpContextAccessor)
      : base(db, db.Articles, httpContextAccessor)
    {
    }
  }

}

// vim:sw=2:ts=2:et:fileformat=dos
