namespace Videotheque.Data
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class Film : AbstractEntity
  {
    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    [Required]
    /// <value>Default value: <code>Price.Gold</code> </value>
    public Price Price { get; set; }

    [Required]
    [DataType(DataType.Date)]
    // Doesn't work:
    // [Range(typeof(DateTime), "4/22/1895", DateTime.UtcNow.ToString(),
    //         ErrorMessage = "Value for {0} must be between {1} and {2}")]
    [FilmReleased]
    /// <summary>Year between 1895 and the Current Year</summary>
    public DateTime ReleaseDate { get; set; }

    // CA1819: "Arrays returned by properties are not write-protected, even if
    //      the property is read-only."
    #pragma warning disable CA1819
    public byte[] Image { get; set; }

    /// <summary>Search films by Title</summary>
    public static Film[] SearchByTitle()
    {
      throw new NotImplementedException();
    }

    /// <summary>Search films by released date</summary>
    public static Film[] SearchByYearDate(string date)
    {
      throw new NotImplementedException();
    }

    /// <summary>Count number of films thare are currently borrowed</summary>
    public int CountBorrowing()
    {
      throw new NotImplementedException();
    }

    /// <summary>Count number of free Articles, not borrowed</summary>
    public int FreeArticles()
    {
      throw new NotImplementedException();
    }

    /// <summary>Return Customers that borrow currently this film</summary>
    public Customer[] Borrowers()
    {
      throw new NotImplementedException();
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
