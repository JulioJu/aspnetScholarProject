namespace Videotheque.Pages.Abstract
{
  using System.Linq;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;

  public abstract partial class CrudAbstract<TAbstractEntity> : PageModel
    where TAbstractEntity : AbstractEntity
  {
    // Detects concurrency exceptions when the one client
    // deletes the movie and the other client posts changes to the movie.
    // The previous code detects concurrency exceptions when the one client
    // deletes the movie and the other client posts changes to the movie.
    private bool AbstractEntityExist(int id)
    {
      return this._tDbSet.Any(e => e.Id == id);
    }

    private protected async Task<IActionResult> OnPostEditAsyncWithFunc(
        PerformTestOverposting peformTestOverposting)
    {
      if (!base.ModelState.IsValid)
      {
        return base.Page();
      }

      this._db.Attach(this.AbstractEntity).State = EntityState.Modified;

      if (await peformTestOverposting())
      {
        try
        {
          await this._db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
          if (!this.AbstractEntityExist(this.AbstractEntity.Id))
          {
            return base.NotFound();
          }
          else
          {
            throw;
          }
        }
      }

      if (!string.IsNullOrEmpty(this.Message))
      {
        this.Message = $"AbstractEntity {this.AbstractEntity.Id} edited. "
          + "<br />Some details: <br /><ul>" + this.Message + "</ul>";
      }
      else
      {
        this.Message = $"AbstractEntity {this.AbstractEntity.Id} edited." +
          this.Message;
      }
      return base.RedirectToPage("./Details/",
          new { id = this.AbstractEntity.Id });
    }

    // public abstract Task<IActionResult> OnPostEditAsync();

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
