namespace Videotheque.Pages.CustomerPage
{
  using Microsoft.AspNetCore.Http;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed class Delete : DeleteAbstract<Customer>
  {
    public Delete(AppDbContext db, IHttpContextAccessor httpContextAccessor)
      : base(db, db.Customers, httpContextAccessor)
    {
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
