namespace Videotheque.Data
{
  using System.ComponentModel.DataAnnotations;

  public class GenreStyle : AbstractEntity
  {
    [Required]
    [MinLengthAttribute(2)]
    [MaxLengthAttribute(100)]
    public GenreStyleEnum Title { get; set; }

    [MinLengthAttribute(2)]
    [MaxLengthAttribute(1000)]
    public string Description { get; set; }

  }

}
// vim:sw=2:ts=2:et:fileformat=dos
