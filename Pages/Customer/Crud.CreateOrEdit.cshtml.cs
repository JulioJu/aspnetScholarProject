namespace Videotheque.Pages.CustomerPage
{
  using System.Collections.Generic;
  using System.Globalization;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages.Abstract;

  public sealed partial class Crud : CrudAbstract<Customer>
  {
    public static readonly int NumberInputArticleToBorrow = 4;

    public List<Article> CurrentlyBorrowedList;

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

    private async Task<Article> RetrieveArticle(string articleIdString)
    {
      int articleId = int.Parse(articleIdString,
          System.Globalization.NumberStyles.Integer,
          CultureInfo.InvariantCulture);
      // INFO
      // If we display Invoice, we don't display keept Articles
      Article currentlyBorrowedArticle = await
        Videotheque.Pages.ArticlePage.Crud.FindArticleAsync(base._db, articleId)
        .ConfigureAwait(false);
      if (currentlyBorrowedArticle != null)
      {
        this.CurrentlyBorrowedList.Add(currentlyBorrowedArticle);
      }
      else
      {
        throw new System.InvalidOperationException("ERROR in POST,"
            + " can't find article with id " + articleId);
      }
      return currentlyBorrowedArticle;
    }

    /// <return>
    ///   Return true if `this.IsInvoice === true` and there is
    ///   an article to remove"
    /// </return>
    private async Task<bool> RemoveSomeArticlesAlreadyBorrowed(
        string[] articleIdAlreadyBorrowedArray,
        string[] shouldBeRemovedArray)
    {
      bool returnValue = false;
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
            Article currentlyBorrowedArticle = await
              this.RetrieveArticle(articleIdAlreadyBorrowedArray[index])
              .ConfigureAwait(false);
            bool shouldBeRemoved = bool.Parse(shouldBeRemovedArray[index]);
            if (currentlyBorrowedArticle.BorrowerId == null)
            {
              throw new System.InvalidOperationException("ERROR in POST"
                  + "ERROR: Article with id '"
                  + currentlyBorrowedArticle.Id
                  + "' is not borrowed by any Customer. "
                  + "Not removed.");
            }
            else
            {
              if (currentlyBorrowedArticle.BorrowerId != base.AbstractEntity.Id)
              {
                  throw new System.InvalidOperationException("ERROR in POST"
                      + "ERROR: Article with id (barcode) '"
                      + currentlyBorrowedArticle.Id
                      + "' not borrowed by the current "
                      + "Customer with id '"
                      + currentlyBorrowedArticle.BorrowerId
                      + "'. Can't be returned "
                      + "by the current Customer.");
              }
              else
              {
                if (shouldBeRemoved)
                {
                  if (!this.IsInvoice)
                  {
                    returnValue = true;
                    continue;
                  }
                  string messageArticle = "Article with barcode"
                    + " '<a href='/Article/Details/"
                    + currentlyBorrowedArticle.Id + "'>"
                    + currentlyBorrowedArticle.Id + "</a>";
                  // 'Microsoft.AspNetCore.Mvc.ViewFeatures.Internal.TempDataSerializer'
                  // cannot serialize an object of type
                  // 'System.Text.StringBuilder'.
                  // S1643: Use a StringBuilder instead.
                  #pragma warning disable S1643
                  base.Message += "<li>" + messageArticle
                      + " is returned.";
                  currentlyBorrowedArticle.BorrowingDate = null;
                  currentlyBorrowedArticle.ReturnDate = null;
                  currentlyBorrowedArticle.BorrowerId = null;

                  // Not needed for Entity Framework Core 2.2, but more clean.
                  // (personal opinion)
                  currentlyBorrowedArticle.Borrower = null;

                  base._db.Attach(currentlyBorrowedArticle).State
                    = EntityState.Modified;
                }
                else
                {
                  System.Console.WriteLine("INFO: Article with id (barcode) '"
                      + currentlyBorrowedArticle.Id
                      + "' is keept by Customer with id '"
                      + currentlyBorrowedArticle.BorrowerId);
                }
              }
            }
          }
          else {
            throw new System.InvalidOperationException("ERROR in POST");
          }
        }

      }
      else
      {
        throw new System.InvalidOperationException("ERROR in POST");
      }
      return returnValue;
    }

    private async Task<bool> AddNewArticlesBorrowed(
        string[] articleIdToBorrowArray,
        string[] articleLoanDurationArray)
    {
      bool isValidationError = false;
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
            // TODO LOW not protected against injections.
            //      All number values could be injected. Could borrow for
            //      all durations.
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

              // Deleted as it should be null
              // base.AbstractEntity.CurrentlyBorrowed.Add(articleToAdd);

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
          else
          {
            // throw new System.InvalidOperationException("ERROR in POST");
          }
        }
      }
      else
      {
        throw new System.InvalidOperationException("ERROR in POST");
      }
      return isValidationError;
    }

    private async Task EdRetrieveAbstractEntity(
        string[] articleIdAlreadyBorrowedArray, string[] shouldBeRemovedArray,
        string[] articleLoanDurationArray, bool doesRetrieveCurrentlyBorrow)
    {
      // VERY IMPORTANT.
      // NO: base.AbstractEntity.CurrentlyBorrowed is lost, should be retrieved
      //    again. But, as we can't know the order, it's done in method
      //    this.RemoveSomeArticlesAlreadyBorrowed()
      // base.AbstractEntity = await
      //   this.PerformSearchInDatabaseFunc(base.AbstractEntity.Id)
      //   .ConfigureAwait(false);
      // this.CurrentlyBorrowed() is also set at null.
      if (doesRetrieveCurrentlyBorrow)
      {
        foreach (string articleIdString in articleIdAlreadyBorrowedArray)
        {
          await this.RetrieveArticle(articleIdString).ConfigureAwait(false);
        }

      }
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

      this.CurrentlyBorrowedList = new List<Article>();

      if (await this.AddNewArticlesBorrowed(articleIdToBorrowArray,
          articleLoanDurationArray).ConfigureAwait(false))
      {
        await this.EdRetrieveAbstractEntity(articleIdAlreadyBorrowedArray,
          shouldBeRemovedArray, articleLoanDurationArray, true);
        return base.Page();
      }
      // We must remove after add, otherwise we sadly could return and article
      // then borrow it again in the same edit.
      bool redirectToInvoice = await
        this.RemoveSomeArticlesAlreadyBorrowed(articleIdAlreadyBorrowedArray,
          shouldBeRemovedArray).ConfigureAwait(false);
      if (!this.IsInvoice && redirectToInvoice)
      {
        await this.EdRetrieveAbstractEntity(articleIdAlreadyBorrowedArray,
            shouldBeRemovedArray, articleLoanDurationArray, false);
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
