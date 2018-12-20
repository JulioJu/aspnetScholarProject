namespace Videotheque.Pages.CustomerPage
{
  using System.Globalization;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed partial class CRUD : CRUDAbstract<Customer>
  {

    private protected override async Task<bool> PerformTestOverpostingFunc()
    {
      return await base.TryUpdateModelAsync<Customer>(
          base.AbstractEntity,
          "customer",   // Prefix for form value.
          s => s.Firstname,
          s => s.Lastname,
          s => s.Society,
          s => s.Address,
          s => s.Phone,
          s => s.Email)
        .ConfigureAwait(false);
    }

    public async override Task<IActionResult> OnPostCreateAsync()
    {
      return await base.OnPostCreateAsyncWithFunc(this.PerformTestOverpostingFunc)
        .ConfigureAwait(false);
    }

    public async override Task<IActionResult> OnPostEditAsync()
    {
      return await base.OnPostEditAsyncWithFunc(this.PerformTestOverpostingFunc)
        .ConfigureAwait(false);
    }

    public async Task<IActionResult> OnPostJoinListAsync()
    {
      return await this.OnPostCreateAsync().ConfigureAwait(false);
    }

    public async Task<IActionResult> OnPostJoinListUCAsync()
    {
      if (!base.ModelState.IsValid)
      {
        return base.Page();
      }
      base.AbstractEntity.Lastname = base.AbstractEntity.Lastname?
        .ToUpper(CultureInfo.CurrentCulture);
      return await this.OnPostJoinListAsync()
        .ConfigureAwait(false);
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
