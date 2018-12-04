namespace Videotheque.Data
{
  using System;
  using System.ComponentModel.DataAnnotations;

  public class Receipt : AbstractEntity
  {
    [Required]
    public int Number { get; set; }

    [Required]
    [DataType(DataType.Date)]
    /// <value>Default value: <code>DateTime.UtcNow</code> </value>
    public DateTime DateTime { get; set; }

    [Required]
    public float TotalPrice { get; set; }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
