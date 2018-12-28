namespace Videotheque.Pages.CustomerPage
{
  using System.Globalization;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
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
          s => s.Email,
          s => s.CurrentlyBorrowed)
        .ConfigureAwait(false);
    }

    public override async Task<IActionResult> OnPostCreateAsync()
    {
      return await base.OnPostCreateAsyncWithFunc(this.PerformTestOverpostingFunc)
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

    public async Task<IActionResult> OnPostEditAsync(
        string[] currentlyBorrowed)
    {
      if (currentlyBorrowed != null)
      {
        foreach (string articleId in currentlyBorrowed)
        {
          if (articleId != null)
          {
            // TODO try catch int.Parse
            int idParsed = int.Parse(articleId);
            Article articleToAdd = await base._db.Articles
                .FindAsync(idParsed);
            if (articleToAdd == null)
            {
              // TODO: display in browser
              System.Console.WriteLine("WARNING: Article with id (barcode) '" +
                  idParsed + "' doesn't exist. Not borrowed.");
            }
            else if (articleToAdd.BorrowerId == null) {
              base.AbstractEntity.CurrentlyBorrowed.Add(articleToAdd);
              base._db.Attach(articleToAdd).State = EntityState.Modified;
              // TODO: display in browser
              System.Console.WriteLine("INFO: Article with id '"
                  + articleToAdd.Id + "' borrowed (added) by Customer with Id '"
                  + articleToAdd.BorrowerId + "'.");
            }
            else
            {
              if (articleToAdd.BorrowerId != AbstractEntity.Id)
              {
                // TODO: display in browser
                System.Console.WriteLine("WARNING: Article with id (barcode) '"
                    + articleToAdd.Id + "' already borrowed by Customer with id '" +
                    articleToAdd.BorrowerId + "'. Can't be borrowed again.");
              }
              else
              {
                // TODO: display in browser
                System.Console.WriteLine("INFO: Article with id (barcode) '"
                    + articleToAdd.Id + "' already borrowed by the current "
                    + "Customer with id '" +
                    articleToAdd.BorrowerId + "'. Can't be borrowed again.");
              }
            }
          }
        }
      }
      return await base.OnPostEditAsyncWithFunc(this.PerformTestOverpostingFunc)
        .ConfigureAwait(false);
    }


  }
}

// vim:sw=2:ts=2:et:fileformat=dos
