namespace Videotheque.Pages.FilmPage
{
  using System.Linq;
  using System.Threading.Tasks;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public class ShowAll : ShowAllAbstract<Film>
  {
    public ShowAll(AppDbContext db)
      : base(db.Films)
    {
    }

    protected private override IQueryable<Film> CompleteQueryable()
    {
      return base.CompleteQueryable()
        .Include(f => f.Articles);
    }

    /// <summary>If variable == -1 or a string, works</summary>
    public async Task OnGetBestBorrowingAsync(int? topN)
    {
      int topNParsed = (topN != null && topN > 0) ? (int)topN : 10000;
      base.AbstractEntities = await this.CompleteQueryable()
        // 'Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryable`1[Videotheque.Data.XXXX]'
        // S2971 : Use 'Count' property here instead.
        #pragma warning disable S2971
        .OrderByDescending(f => f.Articles.Count())
        .Take(topNParsed)
        .ToListAsync()
        ;
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
