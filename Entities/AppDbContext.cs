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
    public DbSet<Customer> Article { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>()
            .HasAlternateKey(a => a.Barcode)
            .HasName("AlternateKey_BareCode");
        modelBuilder.Entity<Article>()
            .Property(a => a.Disc)
            .HasDefaultValue(Conservation.New);
        modelBuilder.Entity<Article>()
            .Property(a => a.Box)
            .HasDefaultValue(Conservation.New);
        modelBuilder.Entity<Article>()
            .Property(a => a.IsLost)
            .HasDefaultValue(false);
        modelBuilder.Entity<Article>()
            .Property(a => a.CountBorrowing)
            .HasDefaultValue(0);
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
