namespace Videotheque.Pages.CustomerPage
{
  using System.Globalization;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages;

  /// <summary>
  /// Manage /Customer/Create and /Customer/Edit/:id
  /// </summary>
  public class Create : Details
  {
    public const int NumberInputArticleToBorrow = 4;

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

    public Create(AppDbContext db)
      : base(db)
    {
    }

    private protected override async Task<bool> PerformTestOverpostingFunc(
        Customer tAbstractEntity)
    {
      return await base.TryUpdateModelAsync<Customer>(
          tAbstractEntity,
          string.Empty,   // Prefix for form value.
          s => s.Firstname,
          s => s.Lastname,
          s => s.Company,
          s => s.AddressStreet,
          s => s.AddressCity,
          s => s.AddressCountry,
          s => s.Phone,
          s => s.Email,
          s => s.Birthdate,
          s => s.IsUnemployed,
          s => s.IsStudent,
          s => s.PeopleWithDisabilities);
    }

    /// <summary>
    /// Try to borrow new articles submited in the corresponding table The user
    /// could pass (thanks developpers tools) any int positive value to
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
    private protected async Task<bool> BorrowArticles(
        Customer customer,
        string[] articleIdToBorrowArray,
        string[] articleLoanDurationArray)
    {
      bool isBorrowed = true;
      this.ArticleIdToBorrowArrayInputValue =
        new string[Create.NumberInputArticleToBorrow];
      this.ValidationMessageArticleIdToBorrowArray =
        new string[Create.NumberInputArticleToBorrow];
      for (int index = 0; index < Create.NumberInputArticleToBorrow;
          index++)
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
          if (articleLoanDuration < 1)
          {
            throw new BadPostRequestException("Param articleLoanDurationArray["
                + index + "] (value " + articleLoanDuration
                + ") must be a number > 0.");
          }
          this.ArticleIdToBorrowArrayInputValue[index] =
            articleId.ToString(CultureInfo.InvariantCulture);

          Article articleToAdd = await Videotheque.Pages
            .ArticlePage.Crud.FindArticleAsyncWithFilm(base._db, articleId);
          if (articleToAdd == null)
          {
            this.ValidationMessageArticleIdToBorrowArray[index] =
              "Article with id (barcode) '"
              + articleId + "' doesn't exist. Not borrowed.";
            isBorrowed = false;
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
            if (articleToAdd.IsLost)
            {
              this.ValidationMessageArticleIdToBorrowArray[index] =
                "Article with id (barcode) '"
                + articleId + "' is lost. Not borrowed.";
              isBorrowed = false;
              continue;
            }
            else if (articleToAdd.Disc == Conservation.Unusable
                || articleToAdd.Box == Conservation.Unusable)
            {
              this.ValidationMessageArticleIdToBorrowArray[index] =
                "Article with id (barcode) '"
                + articleId + "' has the box or the disc unusable. "
                + " Not borrowed.";
              isBorrowed = false;
              continue;
            }
            else if (base.AbstractEntity.IsUnderage()
                && (articleToAdd.Film.GenreStyle == GenreStyleEnum.Pornographic
                    || articleToAdd.Film.GenreStyle
                    == GenreStyleEnum.Violence))
            {
              this.ValidationMessageArticleIdToBorrowArray[index] =
                "Article with id (barcode) '"
                + articleId + "' can't be borrowed by an underage Customer. "
                + "Not borrowed.";
              isBorrowed = false;
              continue;
            }
            articleToAdd.CountBorrowing++;
            articleToAdd.BorrowingDate = System.DateTime.UtcNow;
            articleToAdd.ReturnDate = articleToAdd.BorrowingDate?.AddDays(
                articleLoanDuration);

            articleToAdd.BorrowerId = customer.Id;
            articleToAdd.Borrower = customer;

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
            if (articleToAdd.BorrowerId != base.AbstractEntity?.Id)
            {
              this.ValidationMessageArticleIdToBorrowArray[index] =
                messageArticle + " already borrowed by " + messageBorrower
                + ". Can't be borrowed again.";
              isBorrowed = false;
            }
            else
            {
              this.ValidationMessageArticleIdToBorrowArray[index] =
                messageArticle + " already borrowed by the current "
                  + "Customer. Can't be borrowed again.";
              isBorrowed = false;
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
      return isBorrowed;
    }

    /// <summary>
    /// Retrieve and validate Post param articleLoanDurationArray
    /// </summary>
    /// <exception cref="BadPostRequestException">
    /// For articles listed in the Return form (list hidden inputs), if the
    /// article can't be fetched from Database.
    /// </exception>
    private protected string[] RetrievePostParamArticleLoanDurationArray(
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
          throw new BadPostRequestException("Param articleLoanDurationArray"
              + index + " does not exist");
        }
        articleLoanDurationArray[index] =
          base.Request.Form["articleLoanDurationArray" + index];
      }
      return articleLoanDurationArray;
    }

    /// <param>
    /// articleIdToBorrowArray sent by part of the form to borrow a new article.
    /// Tye input type="number"
    /// </param>
    /// <returns>
    /// <para>
    /// Return <code>base.Page()</code> (url /Customer/Edit/:id)
    /// if the User submit wrong values in the borrow form
    /// </para>
    /// <para>
    /// Return <code>this.BadRequest()</code> (error HTTP 400) in case of
    /// exception <code>BadPostRequestException</code> was catch
    /// </para>
    /// </returns>
    /// <exception cref="BadPostRequestException">
    /// Thrown if POST is malformed or if the Return form is outdated
    /// (in case of concurrence edit).
    /// </exception>
    public async Task<IActionResult> OnPostCreateAsync(
        string[] articleIdToBorrowArray)
    {
      Customer customer = new Customer();

      try
      {
        string[] articleLoanDurationArray;
        articleLoanDurationArray = this
          .RetrievePostParamArticleLoanDurationArray(articleIdToBorrowArray);
        this.ArticleIdToBorrowLoanDurationArray = articleLoanDurationArray;
        if (!await this.BorrowArticles(customer,
              articleIdToBorrowArray,
              articleLoanDurationArray))
        {
          this.Message = string.Empty;
          return base.Page();
        }
      }
      catch (BadPostRequestException e)
      {
        return base.BadRequest(e.Message);
      }

      return await
        base.OnPostCreateAsyncWithFunc(customer);
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
