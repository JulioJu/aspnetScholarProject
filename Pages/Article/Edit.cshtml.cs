namespace Videotheque.Pages.ArticlePage
{
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed class Edit : EditAbstract<Article>
  {
    public Edit(AppDbContext db)
      : base(db, db.Articles)
    {
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
