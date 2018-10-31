// Inspired from https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli

namespace RazorPagesContacts.Data
{
  using System.ComponentModel.DataAnnotations;

  public class Customer
  {
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
