namespace RazorPagesContacts.Data
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class Article
  {
    public int Id { get; set; }

    [Required, StringLength (100)]
    /// <value>Is Unique</value>
    public string Barcode { get; set; }

    [Required]
    /// <value>Default value: <code>Conservation.New</code> </value>
    public Conservation Disc { get; set; }

    [Required]
    /// <value>Default value: <code>Conservation.New</code> </value>
    public Conservation Box { get; set; }

    [Required]
    /// <value>Default value: <code>false</code> </value>
    public bool IsLost { get; set; }

    [Required]
    /// <value>Default value: <code>0</code> </value>
    public int CountBorrowing { get; set; }

    [StringLength(250)]
    public string Comment { get; set; }

    public DateTime BorrowingDate { get; set; }

    public DateTime ReturnDate { get; set; }

    public Customer Borrower { get; set; }

    /// <summary>List all articles borrowed</summary>
    public static Article allArticlesBorrowed() {
      throw new NotImplementedException();
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
