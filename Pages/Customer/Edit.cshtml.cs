namespace Videotheque.Pages.CustomerPage
{
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed class Edit : EditAbstract<Customer>
  {
    public Edit(AppDbContext db)
      : base(db, db.Customers)
    {
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
