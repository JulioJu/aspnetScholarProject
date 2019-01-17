namespace Videotheque.Pages.Abstract
{
  using System.Linq;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;

  public abstract partial class CrudAbstract<TAbstractEntity> : PageModel
    where TAbstractEntity : AbstractEntity, new()
  {
    // Detects concurrency exceptions when the one client
    // deletes the movie and the other client posts changes to the movie.
    // The previous code detects concurrency exceptions when the one client
    // deletes the movie and the other client posts changes to the movie.
    private bool AbstractEntityExist(int id)
    {
      return this._tDbSet.Any(e => e.Id == id);
    }

    // Could not be called OnPostEditAsync(), because
    // if we define OnPostEditAsync(...args) in an inherited function
    // ASP.NET send a message, forbid to define several overloaded
    // OnPostEditAsync
    private protected async Task<IActionResult> OnPostEditAsyncWithFunc()
    {
      if (!base.ModelState.IsValid)
      {
        return base.Page();
      }

      // this._db.Attach(this.AbstractEntity).State = EntityState.Modified;

      TAbstractEntity tAbstractEntity =
        await this._tDbSet.FindAsync(this.AbstractEntity.Id);

      if (await this.PerformTestOverpostingFunc(tAbstractEntity))
      {
        try
        {
          // Otherwise, sometime it bug. Not in official doc, but fix
          // random error
          // Cannot insert explicit value for identity column in table 'Films' when
          //   IDENTITY_INSERT is set to OFF.
          this._db.Attach(tAbstractEntity).State = EntityState.Modified;

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
      else
      {
        throw new BadPostRequestException("TryUpdateModelAsync has failed");
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
