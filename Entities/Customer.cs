namespace Videotheque.Data
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;

  /// <summary>
  ///   Customer entity.
  ///   Either firstname and lastname or company is required
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
    public string Company { get; set; }

    [Required]
    [MinLengthAttribute(2)]
    [MaxLengthAttribute(50)]
    [Display(Name = "Street")]
    public string AddressStreet { get; set; }

    [Required]
    [MinLengthAttribute(2)]
    [MaxLengthAttribute(50)]
    [Display(Name = "City")]
    public string AddressCity { get; set; }

    [MinLengthAttribute(2)]
    [MaxLengthAttribute(50)]
    [Display(Name = "Country")]
    public string AddressCountry { get; set; }

    [Phone]
    public string Phone { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    [DataType(DataType.Date)]
    [CustomerIsCompany]
    [CustomerBirthdate]
    public DateTime? Birthdate { get; set; }

    [CustomerIsCompany]
    [Display(Name = "Is Unemployed")]
    public bool IsUnemployed { get; set; }

    [CustomerIsCompany]
    [Display(Name = "Is Student")]
    public bool IsStudent { get; set; }

    [CustomerIsCompany]
    [Display(Name = "People with Disabilities")]
    public bool PeopleWithDisabilities { get; set; }

    private HashSet<Article> PCurrentlyBorrowed { get; set; }

    [Display(Name = "Currently Borrowed")]
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
      if (this.Birthdate != null && this.Birthdate >
                new DateTime(DateTime.Today.Year - 18,
                  DateTime.Today.Month,
                  DateTime.Today.Day))
      {
        return true;
      }
      return false;
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
