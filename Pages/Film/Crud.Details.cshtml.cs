namespace Videotheque.Pages.FilmPage
{
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed partial class Crud : CrudAbstract<Film>
  {
    public Crud(AppDbContext db)
      : base(db, db.Films)
    {
    }
  }

}

// vim:sw=2:ts=2:et:fileformat=dos
