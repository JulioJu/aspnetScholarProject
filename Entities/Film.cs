namespace Videotheque.Data
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;

  public class Film : AbstractEntity
  {
    [Required]
    [MinLengthAttribute(2)]
    [MaxLengthAttribute(255)]
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

    private HashSet<Article> PArticles { get; set; }

    public HashSet<Article> Articles
    {
      get
      {
        return this.PArticles
          ?? (this.PArticles = new HashSet<Article>());
      }

      set
      {
        this.PArticles = value;
      }
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
