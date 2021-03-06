// From https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli

namespace Videotheque.Pages.Abstract
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;

  public abstract class ShowAllAbstract<TAbstractEntity> : PageModel
    where TAbstractEntity : AbstractEntity
  {
    #pragma warning disable SA1401
    private protected readonly DbSet<TAbstractEntity> _tDbSet;

    public List<TAbstractEntity> AbstractEntities
      { get; private protected set; }

    [TempData]
    public string Message { get; set; }

    private protected ShowAllAbstract(DbSet<TAbstractEntity> tDbSet)
    {
      this._tDbSet = tDbSet;
    }

    protected private virtual IQueryable<TAbstractEntity> CompleteQueryable()
    {
      return this._tDbSet
        .AsNoTracking();
    }

    public async Task OnGetAsync()
    {
      this.AbstractEntities = await this.CompleteQueryable()
        .ToListAsync();
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
