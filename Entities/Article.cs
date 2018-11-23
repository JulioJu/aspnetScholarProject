namespace RazorPagesContacts.Data
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class Article: AbstractEntity
  {
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
    // « The 'bool' property 'IsLost' on entity type 'Article' is configured with
    // a database-generated default. This default will always be used for
    // inserts when the property has the value 'fals e', since this is the CLR
    // default for the 'bool' type. Consider using the nullable 'bool?' type
    // instead so that the default will only be used for inserts when the
    // property value is 'null'. » (source, compilo warning)
    /// <value>Default value: <code>false</code> </value>
    public bool? IsLost { get; set; }

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
