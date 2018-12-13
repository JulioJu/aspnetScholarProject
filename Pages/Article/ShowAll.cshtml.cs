namespace Videotheque.Pages.ArticlePage
{
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public class ShowAll : ShowAllAbstract<Article>
  {
    public ShowAll(AppDbContext db)
      : base(db.Articles)
    {
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
