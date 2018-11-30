namespace Videotheque.Pages.CustomerPage
{
  using System.Globalization;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed class Create : CreateAbstract<Customer>
  {
    public string CurrentRoute { get; }

    // IHttpContextAccessor needs to be injectected in Startup.cs
    public Create(AppDbContext db, IHttpContextAccessor httpContextAccessor)
      : base(db, db.Customers)
    {
      this.CurrentRoute = Microsoft.AspNetCore.Http.Extensions
        .UriHelper.GetEncodedPathAndQuery(
          httpContextAccessor.HttpContext.Request);
      System.Console.WriteLine(this.CurrentRoute);
    }

    public async Task<IActionResult> OnPostJoinListAsync()
    {
      return await this.OnPostAsync().ConfigureAwait(false);
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
