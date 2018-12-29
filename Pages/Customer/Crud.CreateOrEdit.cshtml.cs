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
    // Instantiate in constructor
    public string[] ValidationMessageArticleIdToBorrowArray { get; set; }

    // Instantiate in constructor
    public string[] ArticleIdToBorrowArrayInputValue { get; set; }

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
      // TODO LOW display error
      if (articleIdAlreadyBorrowedArray != null
          && shouldBeRemovedArray != null
          && articleIdAlreadyBorrowedArray.Length
              == shouldBeRemovedArray.Length)
      {
        for (int index = 0;
            index < articleIdAlreadyBorrowedArray.Length;
            index++)
        {
          if (articleIdAlreadyBorrowedArray[index] != null
              && shouldBeRemovedArray[index] != null)
          {
            // TODO LOW try catch int.Parse
            // TODO LOW not protected against injections.
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
              // TODO LOW: display in browser
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
                  // TODO LOW: display in browser
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
                  base.Message += "<li>Article with id (barcode) '"
                      + articleToRemove.Id
                      + "' is returned by Customer with id '"
                      + articleToRemove.BorrowerId + ".</li>";
                  articleToRemove.BorrowingDate = null;
                  articleToRemove.BorrowerId = null;

                  // Not needed for Entity Framework Core 2.2, but more clean.
                  // (personal opinion)
                  articleToRemove.Borrower = null;
                  base.AbstractEntity.CurrentlyBorrowed.Remove(articleToRemove);

                  base._db.Attach(articleToRemove).State = EntityState.Modified;
                }
                else
                {
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

    private async Task<bool> AddNewArticlesBorrowed(
        string[] articleIdToBorrowArray,
        string[] articleLoanDurationArray)
    {
      bool isValidationError = false;
      // TODO LOW display error
      if (articleIdToBorrowArray != null
          && articleLoanDurationArray != null
          && articleIdToBorrowArray.Length == articleLoanDurationArray.Length)
      {
        for (int index = 0; index < articleIdToBorrowArray.Length; index++)
        {
          if (articleIdToBorrowArray[index] != null
              && articleLoanDurationArray[index] != null)
          {
            // TODO LOW try catch int.Parse
            // TODO LOW not protected against injections.
            //      All number values could be injected
            int articleId = int.Parse(articleIdToBorrowArray[index],
                System.Globalization.NumberStyles.Integer,
                CultureInfo.CurrentCulture);
            int articleLoanDuration =
              int.Parse(articleLoanDurationArray[index],
                System.Globalization.NumberStyles.Integer,
                CultureInfo.CurrentCulture);
            this.ArticleIdToBorrowArrayInputValue[index] = articleId.ToString();
            Article articleToAdd = await base._db.Articles
                .FindAsync(articleId).ConfigureAwait(false);
            if (articleToAdd == null)
            {
              this.ValidationMessageArticleIdToBorrowArray[index] =
                "Article with id (barcode) '"
                + articleId + "' doesn't exist. Not borrowed.";
              isValidationError = true;
            }
            else if (articleToAdd.BorrowerId == null)
            {
              articleToAdd.CountBorrowing++;
              articleToAdd.BorrowingDate = System.DateTime.UtcNow;
              articleToAdd.ReturnDate = articleToAdd.BorrowingDate?.AddDays(
                  articleLoanDuration);
              base.AbstractEntity.CurrentlyBorrowed.Add(articleToAdd);

              // Not needed for Entity Framework Core 2.2, but more clean
              // (personal opinion)
              articleToAdd.BorrowerId = base.AbstractEntity.Id;
              articleToAdd.Borrower = base.AbstractEntity;

              base._db.Attach(articleToAdd).State = EntityState.Modified;
              base.Message += "<li>Article with id '"
                  + articleToAdd.Id + "' borrowed (added) by Customer with Id '"
                  + articleToAdd.BorrowerId + "'.</li>";
            }
            else
            {
              if (articleToAdd.BorrowerId != base.AbstractEntity.Id)
              {
                this.ValidationMessageArticleIdToBorrowArray[index] =
                  "Article with id (barcode) '"
                    + articleToAdd.Id
                    + "' already borrowed by Customer with id '"
                    + articleToAdd.BorrowerId + "'. Can't be borrowed again.";
                isValidationError = true;
              }
              else
              {
                this.ValidationMessageArticleIdToBorrowArray[index] =
                  "Article with id (barcode) '"
                    + articleToAdd.Id + "' already borrowed by the current "
                    + "Customer with id '" +
                    articleToAdd.BorrowerId + "'. Can't be borrowed again.";
                isValidationError = true;
              }
            }
          }
        }
      }
      return isValidationError;
    }

    public async Task<IActionResult> OnPostEditAsync(
        string[] articleIdToBorrowArray,
        string[] articleLoanDurationArray,
        string[] articleIdAlreadyBorrowedArray,
        string[] shouldBeRemovedArray)
    {
      if (await this.AddNewArticlesBorrowed(articleIdToBorrowArray,
          articleLoanDurationArray).ConfigureAwait(false)) {
        // Otherwise we lost base.AbstractEntity.CurrentlyBorrowed
        // As it, all is reseted
        base.AbstractEntity = await
          this.PerformSearchInDatabaseFunc(base.AbstractEntity.Id)
          .ConfigureAwait(false);
        this.Message = null;
        return base.Page();
      }
      // We must remove after add, otherwise we sadly could return and article
      // then borrow it again in the same edit.
      await this.RemoveSomeArticlesAlreadyBorrowed(articleIdAlreadyBorrowedArray,
          shouldBeRemovedArray).ConfigureAwait(false);
      return await base.OnPostEditAsyncWithFunc(this.PerformTestOverpostingFunc)
        .ConfigureAwait(false);
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
