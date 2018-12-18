// Inspired from https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli

namespace Videotheque.Pages.Abstract
{
  using System.Linq;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;

  public abstract class CreateOrEditAbstract<TAbstractEntity> :
    DetailsAbstract<TAbstractEntity>
    where TAbstractEntity : AbstractEntity
  {
    [TempData]
    public string Message { get; set; }

    private protected CreateOrEditAbstract(
        AppDbContext db,
        DbSet<TAbstractEntity> tDbSet,
        IHttpContextAccessor httpContextAccessor)
      : base(db, tDbSet, httpContextAccessor)
    {
    }

    private protected delegate Task<bool>
      PerformTestOverposting();

    private protected abstract Task<bool>
      PerformTestOverpostingFunc();

    // Detects concurrency exceptions when the one client
    // deletes the movie and the other client posts changes to the movie.
    // The previous code detects concurrency exceptions when the one client
    // deletes the movie and the other client posts changes to the movie.
    private bool AbstractEntityExist(int id)
    {
      return base._tDbSet.Any(e => e.Id == id);
    }

    private protected async Task<IActionResult>
      OnPostCreateAsyncWithFunc(PerformTestOverposting peformTestOverposting)
    {
      if (!base.ModelState.IsValid)
      {
        return base.Page();
      }

      if (await peformTestOverposting().ConfigureAwait(false))
      {
        this._tDbSet.Add(this.AbstractEntity);
        await this._db.SaveChangesAsync().ConfigureAwait(false);
        this.Message = $"AbstractEntity {this.AbstractEntity.Id} added.";
        return base.RedirectToPage("./ShowAll");
      }

      return null;
    }

    private protected async Task<IActionResult> OnPostEditAsyncWithFunc(
        PerformTestOverposting peformTestOverposting)
    {
      if (!base.ModelState.IsValid)
      {
        return base.Page();
      }

      base._db.Attach(this.AbstractEntity).State = EntityState.Modified;

      if (await peformTestOverposting().ConfigureAwait(false))
      {
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
      }

      this.Message = $"AbstractEntity {this.AbstractEntity.Id} edited.";
      return base.RedirectToPage("./ShowAll");
    }

    public abstract Task<IActionResult> OnPostCreateAsync();

    public abstract Task<IActionResult> OnPostEditAsync();

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
