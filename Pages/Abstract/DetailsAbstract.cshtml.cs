namespace Videotheque.Pages.Abstract
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;

  public abstract class DetailsAbstract<TAbstractEntity> : PageModel
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

    private protected DetailsAbstract(AppDbContext db,
        DbSet<TAbstractEntity> tDbSet)
    {
      this._db = db;
      this._tDbSet = tDbSet;
    }

    private protected delegate
      Task<TAbstractEntity> PerformSearchInDatabase(int? id);

    private protected virtual async
      Task<TAbstractEntity> PerformSearchInDatabaseFunc(int? id)
    {
      return await this._tDbSet
        .FindAsync(id)
        .ConfigureAwait(false);
    }

    // * Could not have two OnGetAsync in a same function, even with
    // parametric overload
    // * PerformSearchInDatabase could not be nullable, because
    // we are in a Generic function.
    // * PerformSearchInDatabase could not be assign to a default
    // callback, default parameter should be evaluated at compiled time.
    private protected async Task<IActionResult> OnGetAsyncWithFunc(int? id,
        PerformSearchInDatabase performSearchInDatabase)
    {
      if (id == null)
      {
        return base.NotFound();
      }

      // FirstOrDefaultAsync is more efficient than SingleOrDefaultAsync at
      // fetching one entity:
      // In much of the scaffolded code, FindAsync can be used in place of
      // FirstOrDefaultAsync.  But if you want to Include other entities, then
      // FindAsync is no longer appropriate.
      // https://docs.microsoft.com/en-us/aspnet/core/data/ef-rp/crud?view=aspnetcore-2.2
      this.AbstractEntity = await performSearchInDatabase(id)
        .ConfigureAwait(false);

      if (this.AbstractEntity == null)
      {
        return base.NotFound();
      }
      return base.Page();
    }

    public virtual async Task<IActionResult> OnGetAsync(int? id)
    {
      return await this.OnGetAsyncWithFunc(id, this.PerformSearchInDatabaseFunc)
        .ConfigureAwait(false);
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos