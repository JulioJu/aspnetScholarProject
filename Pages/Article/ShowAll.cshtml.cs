namespace Videotheque.Pages.ArticlePage
{
  using System.Linq;
  using System.Threading.Tasks;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public class ShowAll : ShowAllAbstract<Article>
  {
    public ShowAll(AppDbContext db)
      : base(db.Articles)
    {
    }

    public override async Task OnGetAsync()
    {
      base.AbstractEntities = await base._tDbSet
        .Include(a => a.Borrower)
        .Include(a => a.Film)
        .AsNoTracking()
        .ToListAsync()
        .ConfigureAwait(false);
    }

    public virtual async Task OnGetCurrentlyBorrowedAsync()
    {
      IQueryable<Article> articles = from a in base._tDbSet
        .Include(a => a.Borrower)
        .Include(a => a.Film)
        where a.BorrowerId != null
        orderby a.ReturnDate
        select a;
      base.AbstractEntities = await articles
        .AsNoTracking()
        .ToListAsync()
        .ConfigureAwait(false);
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
