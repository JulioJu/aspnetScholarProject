namespace Videotheque.Pages.ArticlePage
{
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed class Delete : DeleteAbstract<Article>
  {
    public Delete(AppDbContext db)
      : base(db, db.Articles)
    {
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
