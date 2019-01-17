namespace Videotheque.Pages.Abstract
{
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.RazorPages;
  using Videotheque.Data;

  /// <summary>
  /// Note: for Ends with url Create OnGetAsync defined in file
  /// Pages/Abstract/DetailsAbstract.cs we return Page without perform
  /// search in Database.
  /// </summary>
  public abstract partial class CrudAbstract<TAbstractEntity> : PageModel
    where TAbstractEntity : AbstractEntity, new()
  {
    private protected abstract Task<bool>
      PerformTestOverpostingFunc(TAbstractEntity tAbstractEntity);

    // Could not be called OnPostCreateAsync(), because
    // if we define OnPostCreateAsync(...args) in an inherited function
    // ASP.NET send a message, forbid to define several overloaded
    // OnPostCreateAsync
    private protected async Task<IActionResult>
      OnPostCreateAsyncWithFunc(TAbstractEntity tAbstractEntity)
    {
      if (!base.ModelState.IsValid)
      {
        return base.Page();
      }

      if (await this.PerformTestOverpostingFunc(tAbstractEntity))
      {
        this._tDbSet.Add(tAbstractEntity);
        await this._db.SaveChangesAsync();
        if (!string.IsNullOrEmpty(this.Message))
        {
          this.Message = $"AbstractEntity {tAbstractEntity.Id} created. "
            + "<br />Some details: <br /><ul>" + this.Message + "</ul>";
        }
        else
        {
          this.Message = $"AbstractEntity {tAbstractEntity.Id} created." +
            this.Message;
        }
        return base.RedirectToPage("./Details/",
            new { id = tAbstractEntity.Id });
      }
      else
      {
        throw new BadPostRequestException("TryUpdateModelAsync has failed");
      }

    }

    // public abstract Task<IActionResult> OnPostCreateAsync();

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
