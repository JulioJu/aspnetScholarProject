namespace Videotheque.Pages.Abstract
{
  using System;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;

  public abstract class EditAbstract<TAbstractEntity> : PageModel
    where TAbstractEntity : AbstractEntity
  {
    private readonly AppDbContext _db;

    private readonly DbSet<TAbstractEntity> _tDbSet;

    [BindProperty]
    public TAbstractEntity AbstractEntity { get; set; }

    [ViewData]
    public string Title { get; } = "Edit AbstractEntity —";

    protected EditAbstract(AppDbContext db, DbSet<TAbstractEntity> tDbSet)
    {
      this._db = db;
      this._tDbSet = tDbSet;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
      this.AbstractEntity = await this._tDbSet.FindAsync(id)
        .ConfigureAwait(false);

      if (this.AbstractEntity == null)
      {
        return base.RedirectToPage("../../");
      }

      return base.Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!base.ModelState.IsValid)
      {
        return base.Page();
      }

      this._db.Attach(this.AbstractEntity).State = EntityState.Modified;

      try
      {
        await this._db.SaveChangesAsync().ConfigureAwait(false);
      }
      catch (DbUpdateConcurrencyException)
      {
        #pragma warning disable S112
        throw new Exception(
            $"AbstractEntity {this.AbstractEntity.Id} not found!");
      }

      return base.RedirectToPage("./ShowAll");
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
