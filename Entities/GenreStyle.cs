namespace Videotheque.Data
{
  using System.ComponentModel.DataAnnotations;

  public class GenreStyle : AbstractEntity
  {
    [Required]
    [StringLength(100)]
    public GenreStyleEnum Title { get; set; }

    [StringLength(1000)]
    public string Description { get; set; }

    // CA1819: "Arrays returned by properties are not write-protected, even if
    //      the property is read-only."
    #pragma warning disable CA1819
    public byte[] Image { get; set; }

  }

}
// vim:sw=2:ts=2:et:fileformat=dos
