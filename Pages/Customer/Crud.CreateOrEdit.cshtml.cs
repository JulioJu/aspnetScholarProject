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
    public static readonly int NumberInputArticleToBorrow = 4;

    // Default value: False (due to c# interpretation)
    public bool IsInvoice { get; private set; }

    // CA1819: "Arrays returned by properties are not write-protected, even if
    //      the property is read-only."
    #pragma warning disable CA1819
    public string[] ShouldBeRemovedArray { get; set; }

    // CA1819: "Arrays returned by properties are not write-protected, even if
    //      the property is read-only."
    #pragma warning disable CA1819
    public string[] ArticleIdToBorrowArrayInputValue { get; set; }

    // CA1819: "Arrays returned by properties are not write-protected, even if
    //      the property is read-only."
    #pragma warning disable CA1819
    public string[] ValidationMessageArticleIdToBorrowArray { get; set; }

    // CA1819: "Arrays returned by properties are not write-protected, even if
    //      the property is read-only."
    #pragma warning disable CA1819
    public string[] ArticleIdToBorrowLoanDurationArray { get; set; }

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
        .ToUpper(CultureInfo.InvariantCulture);
      return await this.OnPostJoinListAsync()
        .ConfigureAwait(false);
    }

    /// <return>
    ///   Return true if `this.IsInvoice === true` and there is
    ///   an article to remove"
    /// </return>
    private async Task<bool> RemoveSomeArticlesAlreadyBorrowed(
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
                CultureInfo.InvariantCulture);
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
              // TODO LOW: display in browser
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
                  if (!this.IsInvoice)
                  {
                    return true;
                  }
                  string messageArticle = "Article with barcode"
                    + " '<a href='/Article/Details/"
                    + articleToRemove.Id + "'>" + articleToRemove.Id + "</a>";
                  // 'Microsoft.AspNetCore.Mvc.ViewFeatures.Internal.TempDataSerializer'
                  // cannot serialize an object of type
                  // 'System.Text.StringBuilder'.
                  // S1643: Use a StringBuilder instead.
                  #pragma warning disable S1643
                  base.Message += "<li>" + messageArticle
                      + " is returned.";
                  articleToRemove.BorrowingDate = null;
                  articleToRemove.ReturnDate = null;
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
      return false;
    }

    private async Task<bool> AddNewArticlesBorrowed(
        string[] articleIdToBorrowArray,
        string[] articleLoanDurationArray)
    {
      bool isValidationError = false;
      // TODO LOW display error
      this.ArticleIdToBorrowArrayInputValue =
        new string[Crud.NumberInputArticleToBorrow];
      this.ValidationMessageArticleIdToBorrowArray =
        new string[Crud.NumberInputArticleToBorrow];
      if (articleIdToBorrowArray != null
          && articleLoanDurationArray != null
          && articleIdToBorrowArray.Length == articleLoanDurationArray.Length)
      {
        for (int index = 0; index < Crud.NumberInputArticleToBorrow; index++)
        {
          if (articleIdToBorrowArray[index] != null
              && articleLoanDurationArray[index] != null)
          {
            // TODO LOW try catch int.Parse
            // TODO LOW not protected against injections.
            //      All number values could be injected
            int articleId = int.Parse(articleIdToBorrowArray[index],
                System.Globalization.NumberStyles.Integer,
                CultureInfo.InvariantCulture);
            int articleLoanDuration =
              int.Parse(articleLoanDurationArray[index],
                System.Globalization.NumberStyles.Integer,
                CultureInfo.InvariantCulture);
            this.ArticleIdToBorrowArrayInputValue[index] =
              articleId.ToString(CultureInfo.InvariantCulture);

            Article articleToAdd = await base._db.Articles
                .FindAsync(articleId).ConfigureAwait(false);
            if (articleToAdd == null)
            {
              this.ValidationMessageArticleIdToBorrowArray[index] =
                "Article with id (barcode) '"
                + articleId + "' doesn't exist. Not borrowed.";
              isValidationError = true;
              continue;
            }
            string messageArticle = "Article with barcode"
              + " '<a href='/Article/Details/"
              + articleToAdd.Id + "'>" + articleToAdd.Id + "</a>'";
            string messageBorrower = "Customer with id "
                  + "'<a href='/Customer/Details/"
                  + articleToAdd.BorrowerId + "'>" + articleToAdd.BorrowerId
                  + "</a>'";
            if (articleToAdd.BorrowerId == null)
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
              // 'Microsoft.AspNetCore.Mvc.ViewFeatures.Internal.TempDataSerializer'
              // cannot serialize an object of type
              // 'System.Text.StringBuilder'.
              // S1643: Use a StringBuilder instead.
              #pragma warning disable S1643
              base.Message += "<li>" + messageArticle + " borrowed.</li>";
            }
            else
            {
              if (articleToAdd.BorrowerId != base.AbstractEntity.Id)
              {
                this.ValidationMessageArticleIdToBorrowArray[index] =
                  messageArticle + " already borrowed by " + messageBorrower
                  + ". Can't be borrowed again.";
                isValidationError = true;
              }
              else
              {
                this.ValidationMessageArticleIdToBorrowArray[index] =
                  messageArticle + " already borrowed by the current "
                    + "Customer. Can't be borrowed again.";
                isValidationError = true;
              }
            }
          }
        }
      }
      return isValidationError;
    }

    private async Task EdRetrieveAbstractEntity(string[] shouldBeRemovedArray,
        string[] articleLoanDurationArray)
    {
        // Otherwise we lost base.AbstractEntity.CurrentlyBorrowed
        // As it, all is reseted
        base.AbstractEntity = await
          this.PerformSearchInDatabaseFunc(base.AbstractEntity.Id)
          .ConfigureAwait(false);
        this.Message = null;
        this.IsInvoice = false;
        this.ShouldBeRemovedArray = shouldBeRemovedArray;
        this.ArticleIdToBorrowLoanDurationArray = articleLoanDurationArray;
    }

    public async Task<IActionResult> OnPostEditAsync(
        string[] articleIdAlreadyBorrowedArray,
        string[] articleIdToBorrowArray,
        string isInvoice = "false")
    {
      // TODO LOW try catch bool.Parse
      this.IsInvoice = bool.Parse(isInvoice);

      string[] shouldBeRemovedArray =
        new string[articleIdAlreadyBorrowedArray.Length];
      for (int index = 0; index < articleIdAlreadyBorrowedArray.Length; index++)
      {
        // TODO LOW nothing try catch. If the user change something
        // thanks Developpers tools, we could have an out of bound exception
        shouldBeRemovedArray[index] =
          base.Request.Form["shouldBeRemovedArray" + index];
      }

      string[] articleLoanDurationArray =
        new string[articleIdToBorrowArray.Length];
      for (int index = 0; index < articleIdToBorrowArray.Length; index++)
      {
        // TODO LOW nothing try catch. If the user change something
        // thanks Developpers tools, we could have an out of bound exception
        articleLoanDurationArray[index] =
          base.Request.Form["articleLoanDurationArray" + index];
      }

      if (await this.AddNewArticlesBorrowed(articleIdToBorrowArray,
          articleLoanDurationArray).ConfigureAwait(false))
      {
        await this.EdRetrieveAbstractEntity(shouldBeRemovedArray,
            articleLoanDurationArray).ConfigureAwait(false);
        return base.Page();
      }
      // We must remove after add, otherwise we sadly could return and article
      // then borrow it again in the same edit.
      bool redirectToInvoice = await
        this.RemoveSomeArticlesAlreadyBorrowed(articleIdAlreadyBorrowedArray,
          shouldBeRemovedArray).ConfigureAwait(false);
      if (!this.IsInvoice && redirectToInvoice)
      {
        await this.EdRetrieveAbstractEntity(shouldBeRemovedArray,
            articleLoanDurationArray).ConfigureAwait(false);
        if (base.ModelState.IsValid)
        {
          this.IsInvoice = true;
        }
        return base.Page();
      }
      return await base.OnPostEditAsyncWithFunc(this.PerformTestOverpostingFunc)
        .ConfigureAwait(false);
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
