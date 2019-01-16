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

    protected private override IQueryable<Article> CompleteQueryable()
    {
      return base.CompleteQueryable()
        .Include(a => a.Borrower)
        .Include(a => a.Film)
        .OrderBy(a => a.ReturnDate);
    }

    public async Task OnGetCurrentlyBorrowedAsync()
    {
      base.AbstractEntities = await this.CompleteQueryable()
        .Where(a => a.BorrowerId != null)
        .ToListAsync();
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
