namespace Videotheque.Pages.CustomerPage
{
  using System.Linq;
  using System.Threading.Tasks;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed partial class Crud : CrudAbstract<Customer>
  {
    public Crud(AppDbContext db)
      : base(db, db.Customers)
    {
    }

    private protected async override Task<Customer>
      PerformSearchInDatabaseFunc(int? id)
    {
      Customer customer = await base._tDbSet
        .Include(s => s.CurrentlyBorrowed)
          .ThenInclude(f => f.Film)
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Id == id)
        ;
      string currentRoute = base.HttpContext.Request.Path;
      if (currentRoute.StartsWith("/Customer/Edit/",
            System.StringComparison.InvariantCultureIgnoreCase))
      {
        this.CurrentlyBorrowedList = customer.CurrentlyBorrowed.ToList();
        customer.CurrentlyBorrowed = null;
      }
      return customer;
    }

  }

}

// vim:sw=2:ts=2:et:fileformat=dos
