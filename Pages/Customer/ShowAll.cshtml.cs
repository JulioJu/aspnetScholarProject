namespace Videotheque.Pages.CustomerPage
{
  using System.Linq;
  using System.Threading.Tasks;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public class ShowAll : ShowAllAbstract<Customer>
  {
    public ShowAll(AppDbContext db)
      : base(db.Customers)
    {
    }

    protected private override IQueryable<Customer>
      CompleteQueryable()
    {
      return base.CompleteQueryable()
        .Include(s => s.CurrentlyBorrowed)
          .ThenInclude(f => f.Film)
          // Property 'Int32 Count' is not defined for type
          // 'Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1[Videotheque.Data.XXXX]'
          // S2971 : Use 'Count' property here instead.
          #pragma warning disable S2971
          .OrderBy(f => f.CurrentlyBorrowed.Count());
    }

    public async Task OnGetCurrentBorrowerAsync()
    {
      base.AbstractEntities = await this.CompleteQueryable()
        .Where(f => f.CurrentlyBorrowed.Any())
        .ToListAsync();
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
