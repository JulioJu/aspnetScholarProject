namespace Videotheque.Data
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class Article : AbstractEntity
  {
    [Required]
    /// <value>Default value: <code>Conservation.New</code> </value>
    public Conservation Disc { get; set; }

    [Required]
    /// <value>Default value: <code>Conservation.New</code> </value>
    public Conservation Box { get; set; }

    // Warn during the runtime in the Server Console:
    // « Microsoft.EntityFrameworkCore.Model.Validation[20601]
    //     The 'bool' property 'IsLost' on entity type 'Article' is configured
    //     with a database-generated default. This default will always be used
    //     for inserts when the property has the value 'false', since this is
    //     the CLR default for the 'bool' type. Consider using the nullable
    //     'bool?' type instead so that the default will only be used for
    //     inserts when the property value is 'null'. » (source, compilo
    //     warning) »
    // BUT if we use syntax `bool?', we have the error
    //      at `http://localhost:5000/Article/Create' :
    //      « The IsLost field is required. »
    /// <value>Default value: <code>false</code> </value>
    public bool IsLost { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    /// <value>Default value: <code>0</code> </value>
    [Display(Name = "Count Borrowing")]
    public int CountBorrowing { get; set; } = 0;

    public string Comment { get; set; }

    [Display(Name = "Borrowing Date")]
    public DateTime? BorrowingDate { get; set; }

    public DateTime? ReturnDate { get; set; }

    [Required]
    [Display(Name = "Film Id")]
    public int FilmId { get; set; }

    // should not be required
    public Film Film { get; set; }

    [Display(Name = "Borrower Id")]
    public int? BorrowerId { get; set; }

    public Customer Borrower { get; set; }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
