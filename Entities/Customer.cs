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
    [CustomerName]
    [MinLengthAttribute(2)]
    [MaxLengthAttribute(100)]
    public string Firstname { get; set; }

    [CustomerName]
    [MinLengthAttribute(2)]
    [MaxLengthAttribute(100)]
    public string Lastname { get; set; }

    [CustomerName]
    [MinLengthAttribute(2)]
    [MaxLengthAttribute(100)]
    public string Society { get; set; }

    [Required]
    [MinLengthAttribute(2)]
    [MaxLengthAttribute(50)]
    public string AddressStreet { get; set; }

    [Required]
    [MinLengthAttribute(2)]
    [MaxLengthAttribute(50)]
    public string AddressCity { get; set; }

    [MinLengthAttribute(2)]
    [MaxLengthAttribute(50)]
    public string AddressCountry { get; set; }

    [Phone]
    public string Phone { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [DataType(DataType.Date)]
    [CustomerIsSociety]
    [Birthdate]
    public DateTime? Birthdate { get; set; }

    [CustomerIsSociety]
    public bool IsUnemployed { get; set; }

    [CustomerIsSociety]
    public bool IsStudent { get; set; }

    [CustomerIsSociety]
    public bool PeopleWithDisabilities { get; set; }

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

    public bool IsUnderage()
    {
      if (this.Birthdate < DateTime.Today)
      {
        return true;
      }
      return false;
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
