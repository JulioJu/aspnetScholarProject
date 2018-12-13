// From https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli

namespace Videotheque.Pages.Abstract
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;

  public abstract class ShowAllAbstract<TAbstractEntity> : PageModel
    where TAbstractEntity : AbstractEntity
  {
    private readonly AppDbContext _db;

    private readonly DbSet<TAbstractEntity> _tDbSet;

    public IList<TAbstractEntity> AbstractEntities { get; private set; }

    [TempData]
    public string Message { get; set; }

    protected ShowAllAbstract(AppDbContext db, DbSet<TAbstractEntity> tDbSet)
    {
      this._db = db;
      this._tDbSet = tDbSet;
    }

    public async Task OnGetAsync()
    {
      this.AbstractEntities =
        await this._tDbSet.AsNoTracking().ToListAsync().ConfigureAwait(false);
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
