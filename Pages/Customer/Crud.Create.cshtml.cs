namespace Videotheque.Pages.CustomerPage
{
  using System.Globalization;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages;
  using Videotheque.Pages.Abstract;

  /// <summary>
  /// Manage /Customer/Create and /Customer/Edit/:id
  /// </summary>
  public sealed partial class Crud : CrudAbstract<Customer>
  {
    public static readonly int NumberInputArticleToBorrow = 4;

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
        ;
    }

    /// <summary>
    /// Before display <code>base.Page()</code> instantiate
    /// public properties because all are erased, and not trigger OnGet()
    /// Note: not nead to retrieve again <code>base.AbstractEntity</code>.
    /// (in case of form is not validated or if it's validate and we display the
    /// Invoice).
    /// </summary>
    /// <exception>
    /// See exception raised by this.RetrieveArticle
    /// </exception>
    private async Task ComeBackToPageInstantiatePublicProperties (
        string[] articleIdAlreadyBorrowedArray,
        string[] shouldBeRemovedArray,
        string[] articleLoanDurationArray)
    {
      // VERY IMPORTANT.
      // NO: base.AbstractEntity.CurrentlyBorrowed is lost, should be retrieved
      //    again. But, as we can't know the order, it's done in method
      //    this.ReturnArticles()
      // base.AbstractEntity = await
      //   this.PerformSearchInDatabaseFunc(base.AbstractEntity.Id)
      //   ;
      // this.CurrentlyBorrowed() is also set at null.

      // When we are into url Customer/Edit/:id and in function
      // this.BorrowArticles() (this.ReturnArticles()
      // not triggered)
      if (articleIdAlreadyBorrowedArray != null)
      {
        for (int index = 0;
            index < articleIdAlreadyBorrowedArray.Length;
            index++)
        {
          await this.RetrieveArticle(articleIdAlreadyBorrowedArray[index],
              index);
        }
      }

      this.Message = null;
      this.IsInvoice = false;
      this.ShouldBeRemovedArray = shouldBeRemovedArray;
      this.ArticleIdToBorrowLoanDurationArray = articleLoanDurationArray;
    }

    /// <summary>
    /// Try to borrow new articles submited in the corresponding table The user
    /// could pass (thanks developpers tools) any int value to
    /// <code>articleLoanDurationArray</code>, therefore an article could be
    /// borrowed for any duration.
    /// </summary>
    /// <returns>
    /// Return false if there is something wrong in the input text.  otherwise
    /// return true.
    /// </returns>
    /// <exception cref="BadPostRequestException">
    /// Thrown if POST is malformed : can't cast to int or
    /// <code>articleIdToBorrowArray@(incex)</code> is null
    /// </exception>
    private async Task<bool> BorrowArticles(
        string[] articleIdToBorrowArray,
        string[] articleLoanDurationArray)
    {
      bool isValidationError = false;
      this.ArticleIdToBorrowArrayInputValue =
        new string[Crud.NumberInputArticleToBorrow];
      this.ValidationMessageArticleIdToBorrowArray =
        new string[Crud.NumberInputArticleToBorrow];
      for (int index = 0; index < Crud.NumberInputArticleToBorrow; index++)
      {
        if (articleIdToBorrowArray[index] != null
            && articleLoanDurationArray[index] != null)
        {
          int articleId;
          if (!int.TryParse(articleIdToBorrowArray[index],
              System.Globalization.NumberStyles.Integer,
              CultureInfo.InvariantCulture,
              out articleId))
          {
            throw new BadPostRequestException("Param articleIdToBorrowArray["
                + index + "] (value " + articleIdToBorrowArray[index]
                + ") could not be casted to int.");
          }
          int articleLoanDuration;
          if (!int.TryParse(articleLoanDurationArray[index],
              System.Globalization.NumberStyles.Integer,
              CultureInfo.InvariantCulture,
              out articleLoanDuration))
          {
            throw new BadPostRequestException("Param articleLoanDurationArray["
                + index + "] (value " + articleLoanDurationArray[index]
                + ") could not be casted to int.");
          }
          this.ArticleIdToBorrowArrayInputValue[index] =
            articleId.ToString(CultureInfo.InvariantCulture);

          Article articleToAdd = await base._db.Articles
              .FindAsync(articleId);
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
          if (articleLoanDurationArray[index] == null)
          {
            throw new BadPostRequestException("Param articleLoanDurationArray"
                + index + " should not be null.");
          }
          // articleIdToBorrowArray[index] could be null: nothing to loan
        }
      }
      return isValidationError;
    }

    /// <summary>
    /// Retrieve and validate Post param articleLoanDurationArray
    /// </summary>
    /// <exception cref="BadPostRequestException">
    /// For articles listed in the Return form (list hidden inputs), if the
    /// article can't be fetched from Database.
    /// </exception>
    private string[] RetrievePostParamArticleLoanDurationArray(
        string[] articleIdToBorrowArray)
    {
      string[] articleLoanDurationArray =
        new string[articleIdToBorrowArray.Length];
      for (int index = 0; index < articleIdToBorrowArray.Length; index++)
      {
        if (!base.Request.Form.ContainsKey("articleLoanDurationArray" + index))
        {
          // Note: if articleLoanDurationArray10000 exists
          // and articleIdToBorrowArray.Length == 1 , no problems !
          // the value associated to the key could be null.
          throw new BadPostRequestException("Param articleLoanDurationArray" + index
              + "does not exist");
        }
        articleLoanDurationArray[index] =
          base.Request.Form["articleLoanDurationArray" + index];
      }
      return articleLoanDurationArray;
    }

    public override async Task<IActionResult> OnPostCreateAsync()
    {
      return await base.OnPostCreateAsyncWithFunc(this.PerformTestOverpostingFunc)
        ;
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
