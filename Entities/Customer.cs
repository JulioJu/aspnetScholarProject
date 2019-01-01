namespace Videotheque.Data
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  ///   Customer entity.
  ///   Either firstname and lastname or society is required
  /// </summary>
  public class Customer : AbstractEntity
  {
    [StringLength(100)]
    public string Firstname { get; set; }

    [StringLength(100)]
    public string Lastname { get; set; }

    [StringLength(100)]
    public string Society { get; set; }

    [Required]
    [StringLength(255)]
    public string Address { get; set; }

    [StringLength(100)]
    [Phone]
    public string Phone { get; set; }

    [StringLength(255)]
    [EmailAddress]
    public string Email { get; set; }

    private HashSet<Article> PCurrentlyBorrowed { get; set; }

    public HashSet<Article> CurrentlyBorrowed
    {
      get
      {
        return this.PCurrentlyBorrowed
          ?? (this.PCurrentlyBorrowed = new HashSet<Article>());
      }

      set
      {
        this.PCurrentlyBorrowed = value;
      }
    }

    /// <summary>
    ///     List all custmers that have articles currently borrowed
    /// </summary>
    public static Customer AllBorrowers()
    {
      throw new NotImplementedException();
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
