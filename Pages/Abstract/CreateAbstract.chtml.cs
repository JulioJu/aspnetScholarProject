// Inspired from https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli

namespace Videotheque.Pages.Abstract
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;

  public abstract class CreateAbstract<TAbstractEntity> : PageModel
    where TAbstractEntity : AbstractEntity
  {
    private readonly AppDbContext _db;

    private readonly DbSet<TAbstractEntity> _tDbSet;

    [BindProperty]
    public TAbstractEntity AbstractEntity { get; set; }

    [TempData]
    public string Message { get; set; }

    protected CreateAbstract(AppDbContext db, DbSet<TAbstractEntity> tDbSet)
    {
      this._db = db;
      this._tDbSet = tDbSet;
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!base.ModelState.IsValid)
      {
        return base.Page();
      }

      this._tDbSet.Add(this.AbstractEntity);
      await this._db.SaveChangesAsync().ConfigureAwait(false);
      this.Message = $"AbstractEntity {this.AbstractEntity.Id} added";
      return base.RedirectToPage("./ShowAll");
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos