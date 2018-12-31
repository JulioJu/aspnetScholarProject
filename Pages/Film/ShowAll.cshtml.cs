namespace Videotheque.Pages.FilmPage
{
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public class ShowAll : ShowAllAbstract<Film>
  {
    public ShowAll(AppDbContext db)
      : base(db.Films)
    {
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
