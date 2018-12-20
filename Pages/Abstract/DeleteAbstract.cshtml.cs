namespace Videotheque.Pages.Abstract
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;

  public abstract class DeleteAbstract<TAbstractEntity> :
      DetailsAbstract<TAbstractEntity>
      where TAbstractEntity : AbstractEntity
  {
    protected DeleteAbstract(AppDbContext db,
        DbSet<TAbstractEntity> tDbSet,
        IHttpContextAccessor httpContextAccessor)
      : base(db, tDbSet, httpContextAccessor)
    {
    }

    public async Task<IActionResult> OnPostDeleteAsync(int? id)
    {
      if (id == null)
      {
        return base.NotFound();
      }

      base.AbstractEntity = await base._tDbSet
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Id == id)
        .ConfigureAwait(false);

      if (base.AbstractEntity == null)
      {
        return base.NotFound();
      }
      try
      {
        base._tDbSet.Remove(base.AbstractEntity);
        await base._db.SaveChangesAsync()
          .ConfigureAwait(false);
        return base.RedirectToPage("./ShowAll");
      }
      catch (DbUpdateException /* ex */)
      {
        // Log the error (uncomment ex variable name and write a log.)
        return base.RedirectToAction("./Delete",
            new { id, saveChangesError = true });
      }

    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
