namespace Videotheque.Pages.CustomerPage
{
  using System.Threading.Tasks;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public class Details : CrudAbstract<Customer>
  {
    public Details(AppDbContext db)
      : base(db, db.Customers)
    {
    }

    private protected
      override Task<bool> PerformTestOverpostingFunc(Customer tAbstractEntity)
    {
      throw new System.NotImplementedException();
    }

    private protected async Task<Customer> RetrieveCustomer(int? id)
    {
      return await base._tDbSet
        .Include(s => s.CurrentlyBorrowed)
          .ThenInclude(f => f.Film)
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Id == id);
    }

    private protected async override Task<Customer>
      PerformSearchInDatabaseFunc(int? id)
    {
      Customer customer = await this.RetrieveCustomer(id);
      return customer;
    }

  }

}

// vim:sw=2:ts=2:et:fileformat=dos
