namespace Videotheque.Pages.ArticlePage
{
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed partial class Crud : CrudAbstract<Article>
  {
    public Crud(AppDbContext db)
      : base(db, db.Articles)
    {
    }
  }

}

// vim:sw=2:ts=2:et:fileformat=dos
