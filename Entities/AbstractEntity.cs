namespace Videotheque.Data
{
  using System;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;

  public abstract class AbstractEntity
  {
    public int Id { get; set; }

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [DataType(DataType.Date)]
    public DateTime CreatedDate { get; set; }

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [DataType(DataType.Date)]
    public DateTime UpdatedDate { get; set; }
  }

}

// vim:sw=2:ts=2:et:fileformat=dos
