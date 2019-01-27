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
    [Display(Name = "Release Date")]
    public DateTime ReleaseDate { get; set; }

    [Required]
    [MinLengthAttribute(2)]
    [MaxLengthAttribute(255)]
    [Display(Name = "Directed By")]
    public string DirectedBy { get; set; }

    [Required]
    [Display(Name = "Genre Style")]
    public GenreStyleEnum GenreStyle { get; set; }

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

    private int? _countBorrowingP;

    public int CountBorrowing()
    {
      if (this._countBorrowingP == null)
      {
        this._countBorrowingP = 0;
        foreach (Article article in this.PArticles)
        {
          this._countBorrowingP += article.CountBorrowing;
        }
      }
      int countBorrowing = (int)this._countBorrowingP;
      return countBorrowing;
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
