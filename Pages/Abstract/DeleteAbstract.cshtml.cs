namespace Videotheque.Pages.Abstract
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;

  public abstract class DeleteAbstract<TAbstractEntity> :
      DetailsAbstract<TAbstractEntity>
      where TAbstractEntity : AbstractEntity
  {
    protected DeleteAbstract(AppDbContext db, DbSet<TAbstractEntity> tDbSet)
      : base(db, tDbSet)
    {
    }

    public async Task<IActionResult> OnPostDeleteAsync(int? id)
    {
      if (id == null)
      {
        return base.NotFound();
      }

      base.AbstractEntity = await base._tDbSet.FindAsync(id)
        .ConfigureAwait(false);

      if (base.AbstractEntity != null)
      {
        base._tDbSet.Remove(base.AbstractEntity);
        await base._db.SaveChangesAsync()
          .ConfigureAwait(false);
      }

      return base.RedirectToPage("./ShowAll");
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
