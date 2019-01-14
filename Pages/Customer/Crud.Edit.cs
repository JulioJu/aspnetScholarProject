namespace Videotheque.Pages.CustomerPage
{
  using System.Collections.Generic;
  using System.Globalization;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Videotheque.Data;
  using Videotheque.Pages;
  using Videotheque.Pages.Abstract;

  /// <summary>
  /// Manage only /Customer/Create and /Customer/Edit/:id IMPORTANT see also
  /// the other part of the partial class ./Crud.CreateOrEdit.cshtml.cs
  /// </summary>
  public sealed partial class Crud : CrudAbstract<Customer>
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
      // INFO
      // If we display Invoice, we don't display keept Articles
      Article currentlyBorrowedArticle = await
        Videotheque.Pages.ArticlePage.Crud.FindArticleAsync(base._db,
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
        string[] articleIdToBorrowArray,
        string isInvoice = "false")
    {
      // 1) Test and cast POST params and instantiate corresponding vars
      // =========================
      // =========================
      bool isInvoiceTmp;
      if (!bool.TryParse(isInvoice, out isInvoiceTmp))
      {
        throw new BadPostRequestException("Param isInvoice "
            + "(value " + isInvoice + ") could not be casted to bool.");
      }
      this.IsInvoice = isInvoiceTmp;

      string[] shouldBeRemovedArray =
        new string[articleIdAlreadyBorrowedArray.Length];
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

      string[] articleLoanDurationArray;

      articleLoanDurationArray = this
        .RetrievePostParamArticleLoanDurationArray(articleIdToBorrowArray);

      this.CurrentlyBorrowedList = new List<Article>();

      // 2) Perform borrowing, then return
      // =============================
      // =============================

      // BORROWING
      if (await this.BorrowArticles(articleIdToBorrowArray,
            articleLoanDurationArray))
      {
        await this.ComeBackToPageInstantiatePublicProperties(
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
          await this.ComeBackToPageInstantiatePublicProperties(
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
    /// <code>base.Model.IsValid === false</code> OR exception
    /// <code>BadPostRequestException</code> was catch
    /// </para>
    /// </returns>
    /// <exception cref="BadPostRequestException">
    /// Thrown if POST is malformed or if the Return form is outdated
    /// (in case of concurrence edit).
    /// </exception>
    public async Task<IActionResult> OnPostEditAsync(
        string[] articleIdAlreadyBorrowedArray,
        string[] articleIdToBorrowArray,
        string isInvoice = "false")
    {
      try
      {
        if (!await this.PerformBorrowAndReturn(articleIdAlreadyBorrowedArray,
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
      return await base
          .OnPostEditAsyncWithFunc(this.PerformTestOverpostingFunc);
    }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
