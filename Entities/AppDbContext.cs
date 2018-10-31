// Inspired from https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli

namespace RazorPagesContacts.Data
{
  using Microsoft.EntityFrameworkCore;

  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions options)
      : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
  }
}

// vim:sw=2:ts=2:et:fileformat=dos
