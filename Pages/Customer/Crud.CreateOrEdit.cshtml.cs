namespace Videotheque.Pages.CustomerPage
{
  using System.Globalization;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed partial class Crud : CrudAbstract<Customer>
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
        string[] articleIdBorrowedArray, string[] articleLoanDurationArray)
    {
      // TODO display error
      if (articleIdBorrowedArray != null
          && articleLoanDurationArray != null
          && articleIdBorrowedArray.Length == articleLoanDurationArray.Length
          )
      {
        for (int index = 0 ; index < articleIdBorrowedArray.Length ; index++)
        {
          if (articleIdBorrowedArray[index] != null
              && articleLoanDurationArray[index] != null)
          {
            // TODO try catch int.Parse
            // TODO not protected against injections.
            //      All number values could be injected
            int articleId = int.Parse(articleIdBorrowedArray[index],
                System.Globalization.NumberStyles.Integer,
                CultureInfo.CurrentCulture);
            int articleLoanDuration =
              int.Parse(articleLoanDurationArray[index],
                System.Globalization.NumberStyles.Integer,
                CultureInfo.CurrentCulture);
            Article articleToAdd = await base._db.Articles
                .FindAsync(articleId).ConfigureAwait(false);
            if (articleToAdd == null)
            {
              // TODO: display in browser
              System.Console.WriteLine("WARNING: Article with id (barcode) '" +
                  articleId + "' doesn't exist. Not borrowed.");
            }
            else if (articleToAdd.BorrowerId == null)
            {
              articleToAdd.CountBorrowing++;
              articleToAdd.BorrowingDate = System.DateTime.UtcNow;
              articleToAdd.ReturnDate = articleToAdd.BorrowingDate?.AddDays(
                  articleLoanDuration);
              base.AbstractEntity.CurrentlyBorrowed.Add(articleToAdd);
              base._db.Attach(articleToAdd).State = EntityState.Modified;
              // TODO: display in browser
              System.Console.WriteLine("INFO: Article with id '"
                  + articleToAdd.Id + "' borrowed (added) by Customer with Id '"
                  + articleToAdd.BorrowerId + "'.");
            }
            else
            {
              if (articleToAdd.BorrowerId != base.AbstractEntity.Id)
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
