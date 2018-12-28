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

    private async Task RemoveSomeArticlesAlreadyBorrowed(
        string[] articleIdAlreadyBorrowedArray,
        string[] shouldBeRemovedArray)
    {
      // TODO display error
      if (articleIdAlreadyBorrowedArray != null
          && shouldBeRemovedArray != null
          && articleIdAlreadyBorrowedArray.Length
              == shouldBeRemovedArray.Length)
      {
        for (int index = 0 ;
            index < articleIdAlreadyBorrowedArray.Length ;
            index++)
        {
          if (articleIdAlreadyBorrowedArray[index] != null
              && shouldBeRemovedArray[index] != null)
          {
            // TODO try catch int.Parse
            // TODO not protected against injections.
            //      All number values could be injected
            int articleId = int.Parse(articleIdAlreadyBorrowedArray[index],
                System.Globalization.NumberStyles.Integer,
                CultureInfo.CurrentCulture);
            System.Console.WriteLine(shouldBeRemovedArray[index]);
            bool shouldBeRemoved = bool.Parse(shouldBeRemovedArray[index]);
            Article articleToRemove = await base._db.Articles
                .FindAsync(articleId).ConfigureAwait(false);
            if (articleToRemove == null)
            {
              // TODO: display in browser
              System.Console.WriteLine("WARNING: Article with id (barcode) '"
                  + articleId + "' doesn't exist. Not removed.");
            }
            else if (articleToRemove.BorrowerId == null)
            {
              System.Console.WriteLine("WARNING: Article with id '"
                  + articleToRemove.Id + "' is not borrowed by any Customer. "
                  + "Not removed.");
            }
            else
            {
              if (articleToRemove.BorrowerId != base.AbstractEntity.Id)
              {
                  // TODO: display in browser
                  System.Console.WriteLine("INFO: Article with id (barcode) '"
                      + articleToRemove.Id + "' not borrowed by the current "
                      + "Customer with id '"
                      + articleToRemove.BorrowerId + "'. Can't be returned "
                      + "by the current Customer.");
              }
              else
              {
                if (shouldBeRemoved)
                {
                  // TODO: display in browser
                  System.Console.WriteLine("INFO: Article with id (barcode) '"
                      + articleToRemove.Id
                      + "' is returned by Customer with id '"
                      + articleToRemove.BorrowerId);
                  articleToRemove.BorrowingDate = null;
                  articleToRemove.BorrowerId = null;
                  base._db.Attach(articleToRemove).State = EntityState.Modified;
                }
                else {
                  System.Console.WriteLine("INFO: Article with id (barcode) '"
                      + articleToRemove.Id + "' is keeped by Customer with id '"
                      + articleToRemove.BorrowerId);
                }
              }
            }
          }
        }

      }
    }

    private async Task AddNewArticlesBorrowed(
        string[] articleIdToBorrowArray,
        string[] articleLoanDurationArray) {
      // TODO display error
      if (articleIdToBorrowArray != null
          && articleLoanDurationArray != null
          && articleIdToBorrowArray.Length == articleLoanDurationArray.Length
          )
      {
        for (int index = 0 ; index < articleIdToBorrowArray.Length ; index++)
        {
          if (articleIdToBorrowArray[index] != null
              && articleLoanDurationArray[index] != null)
          {
            // TODO try catch int.Parse
            // TODO not protected against injections.
            //      All number values could be injected
            int articleId = int.Parse(articleIdToBorrowArray[index],
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
              System.Console.WriteLine("WARNING: Article with id (barcode) '"
                  + articleId + "' doesn't exist. Not borrowed.");
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
                    + articleToAdd.Id
                    + "' already borrowed by Customer with id '"
                    + articleToAdd.BorrowerId + "'. Can't be borrowed again.");
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
    }

    public async Task<IActionResult> OnPostEditAsync(
        string[] articleIdToBorrowArray,
        string[] articleLoanDurationArray,
        string[] articleIdAlreadyBorrowedArray,
        string[] shouldBeRemovedArray)
    {
      await this.AddNewArticlesBorrowed(articleIdToBorrowArray,
          articleLoanDurationArray);
      await this.RemoveSomeArticlesAlreadyBorrowed(articleIdAlreadyBorrowedArray,
          shouldBeRemovedArray);
      return await base.OnPostEditAsyncWithFunc(this.PerformTestOverpostingFunc)
        .ConfigureAwait(false);
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
