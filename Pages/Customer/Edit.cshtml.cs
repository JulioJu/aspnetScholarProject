namespace Videotheque.Pages.CustomerPage
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages;

  /// <summary>
  /// Manage only /Customer/Create and /Customer/Edit/:id IMPORTANT see also
  /// the other part of the partial class ./Crud.CreateOrEdit.cshtml.cs
  /// </summary>
  public class Edit : Create
  {
    /// <value>
    /// Default value: False (due to c# interpretation) Set to true to display
    /// the Invoice and not the form
    /// </value>
    public bool IsInvoice { get; private set; }

    // CA1819: "Arrays returned by properties are not write-protected, even if
    //      the property is read-only."
    #pragma warning disable CA1819
    public string[] ShouldBeRemovedArray { get; set; }

    public List<Article> CurrentlyBorrowedList { get; private set; }

    /// <summary> Used only when we display Invoice </summary>
    public int CustomDiscount { get; private set; }

    public Edit(AppDbContext db)
      : base(db)
    {
    }

    private protected async override Task<Customer>
      PerformSearchInDatabaseFunc(int? id)
    {
      Customer customer = await base.RetrieveCustomer(id);
      string currentRoute = base.HttpContext.Request.Path;
      if (currentRoute.StartsWith("/Customer/Edit/",
            System.StringComparison.InvariantCultureIgnoreCase))
      {
        this.CurrentlyBorrowedList = customer.CurrentlyBorrowed.ToList();
        customer.CurrentlyBorrowed = null;
      }
      return customer;
    }

    /// <summary> Retrieve an article saved in Database </summary>
    /// <exception cref="BadPostRequestException">
    /// if the article can't be find.
    /// </exception>
    private async Task<Article> RetrieveArticle(string articleIdString,
        int index)
    {
      int articleId;
      if (!int.TryParse(articleIdString,
          System.Globalization.NumberStyles.Integer,
          CultureInfo.InvariantCulture,
          out articleId))
      {
        throw new BadPostRequestException("Param articleIdAlreadyBorrowedArray["
            + index + "] (value " + articleIdString
            + ") could not be casted to int.");
      }
      Article currentlyBorrowedArticle = await
        Videotheque.Pages.ArticlePage.Crud.FindArticleAsyncWithFilm(base._db,
                articleId);
      if (currentlyBorrowedArticle != null)
      {
        this.CurrentlyBorrowedList.Add(currentlyBorrowedArticle);
      }
      else
      {
        throw new BadPostRequestException("Can't find article with id "
            + articleId + ". Param articleIdAlreadyBorrowedArray["
            + index + "] is wrong.");
      }
      return currentlyBorrowedArticle;
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
    private async Task InstantiatePublicProperties (
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

      if (articleIdAlreadyBorrowedArray != null)
      {
        for (int index = 0;
            index < articleIdAlreadyBorrowedArray.Length;
            index++)
        {
          await this.RetrieveArticle(articleIdAlreadyBorrowedArray[index], index);
        }
      }

      this.Message = null;
      this.IsInvoice = false;
      this.ShouldBeRemovedArray = shouldBeRemovedArray;
      this.ArticleIdToBorrowLoanDurationArray = articleLoanDurationArray;
    }

    /// <summary>
    /// Try to return article already borrowed submited in the corresponding
    /// table
    /// </summary>
    /// <returns>
    /// Return true if `this.IsInvoice === true` and there is
    /// an article to remove"
    /// </returns>
    /// <exception cref="BadPostRequestException">
    /// <para>
    /// 1) Thrown if POST is malformed : can't cast to int or bool
    /// </para>
    /// <para>
    /// 2) Thrown if the article is not currently borrowed by the current user
    /// (different messages for each case) or if the article can't be fetched
    /// from Database.
    /// </para>
    /// </exception>
    private async Task<bool> ReturnArticles(
        string[] articleIdAlreadyBorrowedArray,
        string[] shouldBeRemovedArray)
    {
      bool returnValue = false;
      for (int index = 0;
          index < articleIdAlreadyBorrowedArray.Length;
          index++)
      {
        if (articleIdAlreadyBorrowedArray[index] != null
            && shouldBeRemovedArray[index] != null)
        {
          Article currentlyBorrowedArticle = await
            this.RetrieveArticle(articleIdAlreadyBorrowedArray[index], index);

          bool shouldBeRemoved;
          if (!bool.TryParse(shouldBeRemovedArray[index], out shouldBeRemoved))
          {
            throw new BadPostRequestException("Param shouldBeRemovedArray["
                + index + "] (value " + shouldBeRemovedArray[index]
                + ") could not be casted to bool.");
          }

          if (currentlyBorrowedArticle.BorrowerId == null)
          {
            throw new BadPostRequestException("Article with id '"
                + currentlyBorrowedArticle.Id
                + "' is not borrowed by any Customer. "
                + "Not removed. Param articleIdAlreadyBorrowedArray["
                + index + "] not valid.");
          }
          else
          {
            if (currentlyBorrowedArticle.BorrowerId != base.AbstractEntity.Id)
            {
                throw new BadPostRequestException("Article with id (barcode) '"
                    + currentlyBorrowedArticle.Id
                    + "' not borrowed by the current "
                    + "Customer with id '"
                    + currentlyBorrowedArticle.BorrowerId
                    + "'. Can't be returned "
                    + "by the current Customer. Param " +
                    " articleIdAlreadyBorrowedArray["
                    + index + "] not valid.");
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
        else
        {
            throw new BadPostRequestException("Params "
              + "articleIdAlreadyBorrowedArray[" + index
              + "] and shouldBeRemovedArray" + index
              + " should not be null.");
        }
      }

      return returnValue;
    }

    /// <returns>
    /// Return false
    /// <para>
    /// 1) if the User submit wrong values in the borrow form
    /// </para>
    /// <para>
    /// 2) if there is articles to borrow AND the Invoice is not still displayed
    /// (this.IsInvoice === false).  In this cas, this.IsInvoice become true,
    /// then the next time the user will click to the submit button we will
    /// return the /Customer/Details/:id page instead of the /Customer/Edit/:id
    /// page.
    /// </para>
    /// </returns>
    /// <exception cref="BadPostRequestException">
    ///  Thrown if POST is malformed or if the Return form is outdated.  For
    ///  instance articles listed in the Return form (list hidden inputs), if
    ///  the article can't be fetched from Database.
    /// </exception>
    private async Task<bool> PerformBorrowAndReturn(
        string[] articleIdAlreadyBorrowedArray,
        string customDiscount,
        string[] articleIdToBorrowArray,
        string isInvoice = "false")
    {
      string[] shouldBeRemovedArray = Array.Empty<string>();
      // 1) Test and cast POST params and instantiate corresponding vars
      // =========================
      // =========================

      this.CurrentlyBorrowedList = new List<Article>();
      if (articleIdAlreadyBorrowedArray.Length > 0)
      {
        bool isInvoiceTmp;
        if (!bool.TryParse(isInvoice, out isInvoiceTmp))
        {
          throw new BadPostRequestException("Param isInvoice "
              + "(value " + isInvoice + ") could not be casted to bool.");
        }
        this.IsInvoice = isInvoiceTmp;

        try
        {
          this.CustomDiscount = int.Parse(customDiscount,
              System.Globalization.NumberStyles.Integer,
              CultureInfo.InvariantCulture);
        }
        catch (System.Exception)
        {
          throw new BadPostRequestException("Param customDiscount"
              + "(value " + customDiscount
              + ") could not be casted to int.");
        }
        if (this.CustomDiscount < 0)
        {
          throw new BadPostRequestException("Param customDiscount (value "
              + customDiscount + ") must be a number >= 0.");
        }
        if (this.CustomDiscount > 99)
        {
          throw new BadPostRequestException("Param customDiscount (value "
              + customDiscount + ") must be a number < 99.");
        }

        shouldBeRemovedArray = new string[articleIdAlreadyBorrowedArray.Length];
        if (articleIdAlreadyBorrowedArray.Length > 0)
        {
          for (int index = 0; index < articleIdAlreadyBorrowedArray.Length; index++)
          {
            if (!base.Request.Form.ContainsKey("shouldBeRemovedArray" + index))
            {
              // Note: if shouldBeRemovedArray10000 exists
              // and articleIdAlreadyBorrowedArray.Length == 1 , no problems !
              // the value associated to the key could be null.
              throw new BadPostRequestException("Param shouldBeRemovedArray"
                  + index + "does not exist");
            }
            shouldBeRemovedArray[index] =
              base.Request.Form["shouldBeRemovedArray" + index];
          }
        }
      }

      string[] articleLoanDurationArray;
      articleLoanDurationArray = base
        .RetrievePostParamArticleLoanDurationArray(articleIdToBorrowArray);

      // 2) Perform borrowing, then return
      // =============================
      // =============================

      // BORROWING
      if (!await base.BorrowArticles(this.AbstractEntity,
            articleIdToBorrowArray,
            articleLoanDurationArray))
      {
        await this.InstantiatePublicProperties(
            articleIdAlreadyBorrowedArray,
            shouldBeRemovedArray,
            articleLoanDurationArray);
        return false;
      }

      // RETURN
      // We must return after borrowing, otherwise we sadly could return and article
      // then borrow it again in the same edit.
      if (articleIdAlreadyBorrowedArray.Length > 0)
      {
        bool redirectToInvoice = false;
        redirectToInvoice = await
            this.ReturnArticles(articleIdAlreadyBorrowedArray,
                    shouldBeRemovedArray);

        if (!this.IsInvoice && redirectToInvoice)
        {
          await this.InstantiatePublicProperties(
              null,
              shouldBeRemovedArray,
              articleLoanDurationArray);
          if (base.ModelState.IsValid)
          {
            this.IsInvoice = true;
          }
          return false;
        }
      }
      else
      {
        if (!base.ModelState.IsValid)
        {
          await this.InstantiatePublicProperties(
              null,
              shouldBeRemovedArray,
              articleLoanDurationArray);
          return false;
        }
      }

      return true;

    }

    /// <param>
    /// articleIdAlreadyBorrowedArray sent by part of the form
    /// to keep or return articles already borrowed. It's an hidden input.
    /// </param>
    /// <param>
    /// articleIdToBorrowArray sent by part of the form to borrow a new article.
    /// Tye input type="number"
    /// </param>
    /// <param>
    /// isInvoice with value true by the form when the Invoice is displayed
    /// in client side
    /// (when <code>this.IsInvoice == true</code> when the view is build).
    /// </param>
    /// <summary>
    /// <para>
    /// Don't forget that the form could be seen as three independants parts.
    /// 1) The part with base fields of customers. 2) The Return table that
    /// displays Articles already borrowed and that could be returned or kept.
    /// 3) The table to borrow news articles.
    /// </para>
    /// <para>
    /// Don't forget that this function display an Invoice if
    /// some Articles are returned and if this.IsInvoice become true
    /// </para>
    /// <para>
    /// When this.Invoice == true, all checks are performed again. We could
    /// display error if some values are not still correct, like if an article
    /// is already returned or if a customer has borrowed an article in an other
    /// tab.
    /// </para>
    /// </summary>
    /// <param>
    /// </param>
    /// <returns>
    /// <para>
    /// Return <code>base.Page()</code> (url /Customer/Edit/:id)
    /// 1) if the User submit wrong values in the borrow form (this.IsInvoice
    /// stay false)
    /// 2) if base.Model.IsValid === false (this.IsInvoice stay false)
    /// 3) if there is articles to borrow AND the Invoice is not still display
    /// (<code>this.IsInvoice === false</code>). In this cas, this.IsInvoice become true.
    /// THEREFORE:
    /// </para>
    /// <para>
    /// Return url /Customer/Details/:id
    /// the next time the user will click to the submit button we will return
    /// the /Customer/Details/:id page instead of the /Customer/Edit/:id page.
    /// </para>
    /// <para>
    /// Return <code>this.BadRequest()</code> (error HTTP 400) in case of
    ///  exception <code>BadPostRequestException</code> was catch
    /// </para>
    /// </returns>
    /// <exception cref="BadPostRequestException">
    /// Thrown if POST is malformed or if the Return form is outdated
    /// (in case of concurrence edit).
    /// </exception>
    public async Task<IActionResult> OnPostEditAsync(
        string[] articleIdAlreadyBorrowedArray,
        string customDiscount,
        string[] articleIdToBorrowArray,
        string isInvoice = "false")
    {
      try
      {
        if (!await this.PerformBorrowAndReturn(articleIdAlreadyBorrowedArray,
              customDiscount,
              articleIdToBorrowArray,
              isInvoice))
        {
          return base.Page();
        }
      }
      catch (BadPostRequestException e)
      {
        return base.BadRequest(e.Message);
      }
      return await base.OnPostEditAsyncWithFunc();
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
