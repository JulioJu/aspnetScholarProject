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
    where TAbstractEntity : AbstractEntity
  {
    private protected delegate Task<bool>
      PerformTestOverposting();

    private protected abstract Task<bool>
      PerformTestOverpostingFunc();

    private protected async Task<IActionResult>
      OnPostCreateAsyncWithFunc(PerformTestOverposting peformTestOverposting)
    {
      if (!base.ModelState.IsValid)
      {
        return base.Page();
      }

      if (await peformTestOverposting())
      {
        this._tDbSet.Add(this.AbstractEntity);
        await this._db.SaveChangesAsync();
        if (!string.IsNullOrEmpty(this.Message))
        {
          this.Message = $"AbstractEntity {this.AbstractEntity.Id} created. "
            + "<br />Some details: <br /><ul>" + this.Message + "</ul>";
        }
        else
        {
          this.Message = $"AbstractEntity {this.AbstractEntity.Id} created." +
            this.Message;
        }
        return base.RedirectToPage("./Details/",
            new { id = this.AbstractEntity.Id });
      }

      return null;
    }

    // public abstract Task<IActionResult> OnPostCreateAsync();

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
