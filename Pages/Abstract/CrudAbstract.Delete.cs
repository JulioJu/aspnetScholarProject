namespace Videotheque.Pages.Abstract
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;

  public abstract partial class CrudAbstract<TAbstractEntity> : PageModel
      where TAbstractEntity : AbstractEntity
  {
    public async Task<IActionResult> OnPostDeleteAsync(int? id)
    {
      if (id == null)
      {
        return base.NotFound();
      }

      this.AbstractEntity = await this._tDbSet
        .AsNoTracking()
        .FirstOrDefaultAsync(m => m.Id == id);

      if (this.AbstractEntity == null)
      {
        return base.NotFound();
      }
      try
      {
        this._tDbSet.Remove(this.AbstractEntity);
        await this._db.SaveChangesAsync();
        this.Message = this.AbstractEntity.GetType() +
            $"{this.AbstractEntity.Id} deleted.";
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
