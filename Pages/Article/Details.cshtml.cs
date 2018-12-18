namespace Videotheque.Pages.ArticlePage
{
  using Microsoft.AspNetCore.Http;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed class Details : DetailsAbstract<Article>
  {
    public Details(AppDbContext db, IHttpContextAccessor httpContextAccessor)
      : base(db, db.Articles, httpContextAccessor)
    {
    }
  }

}

// vim:sw=2:ts=2:et:fileformat=dos
