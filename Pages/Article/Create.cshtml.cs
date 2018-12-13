namespace Videotheque.Pages.ArticlePage
{
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed class Create : CreateAbstract<Article>
  {
    // IHttpContextAccessor needs to be injectected in Startup.cs
    public Create(AppDbContext db)
      : base(db, db.Articles)
    {
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
