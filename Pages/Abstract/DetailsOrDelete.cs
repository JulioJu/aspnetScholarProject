namespace Videotheque.Pages.Abstract
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;

  public abstract class DetailsOrDeleteAbstract<TAbstractEntity> : PageModel
      where TAbstractEntity : AbstractEntity
  {
    // SA1401: Field must be private
    #pragma warning disable SA1401
    private protected readonly AppDbContext _db;

    // SA1401: Field must be private
    #pragma warning disable SA1401
    private protected readonly DbSet<TAbstractEntity> _tDbSet;

    [BindProperty]
    public TAbstractEntity AbstractEntity { get; set; }

    private protected DetailsOrDeleteAbstract(AppDbContext db,
        DbSet<TAbstractEntity> tDbSet)
    {
      this._db = db;
      this._tDbSet = tDbSet;
    }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
      if (id == null)
      {
        return base.NotFound();
      }

      this.AbstractEntity = await this._tDbSet
        .FirstOrDefaultAsync(m => m.Id == id)
        .ConfigureAwait(false);

      if (this.AbstractEntity == null)
      {
        return base.NotFound();
      }
      return base.Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int? id)
    {
      if (id == null)
      {
        return base.NotFound();
      }

      this.AbstractEntity = await this._tDbSet.FindAsync(id)
        .ConfigureAwait(false);

      if (this.AbstractEntity != null)
      {
        this._tDbSet.Remove(this.AbstractEntity);
        await this._db.SaveChangesAsync()
          .ConfigureAwait(false);
      }

      return base.RedirectToPage("./ShowAll");
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
