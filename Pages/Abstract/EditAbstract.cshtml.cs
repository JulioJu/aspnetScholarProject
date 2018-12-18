namespace Videotheque.Pages.Abstract
{
  using System.Linq;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;

  public abstract class EditAbstract<TAbstractEntity> :
      DetailsAbstract<TAbstractEntity>
      where TAbstractEntity : AbstractEntity
  {
    protected EditAbstract(AppDbContext db, DbSet<TAbstractEntity> tDbSet)
      : base(db, tDbSet)
    {
    }

    // Detects concurrency exceptions when the one client
    // deletes the movie and the other client posts changes to the movie.
    // The previous code detects concurrency exceptions when the one client
    // deletes the movie and the other client posts changes to the movie.
    private bool AbstractEntityExist(int id)
    {
      return base._tDbSet.Any(e => e.Id == id);
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (!base.ModelState.IsValid)
      {
        return base.Page();
      }

      base._db.Attach(this.AbstractEntity).State = EntityState.Modified;

      try
      {
        await base._db.SaveChangesAsync().ConfigureAwait(false);
      }
      catch (DbUpdateConcurrencyException)
      {
        if (!this.AbstractEntityExist(base.AbstractEntity.Id))
        {
          return base.NotFound();
        }
        else
        {
          throw;
        }
      }

      return base.RedirectToPage("./ShowAll");
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos