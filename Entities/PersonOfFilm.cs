namespace Videotheque.Data
{
  using System.ComponentModel.DataAnnotations;

  public class PersonOfFilm : AbstractEntity
  {
    [Required]
    [MinLengthAttribute(2)]
    [MaxLengthAttribute(100)]
    public string Firstname { get; set; }

    [Required]
    [MinLengthAttribute(2)]
    [MaxLengthAttribute(100)]
    public string Lastname { get; set; }

    [MinLengthAttribute(2)]
    [MaxLengthAttribute(100)]
    public string Surname { get; set; }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
