// Inspired from https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.1&tabs=netcore-cli

namespace RazorPagesContacts.Data
{
  using Microsoft.EntityFrameworkCore;
  using System;
  // https://stackoverflow.com/questions/36798186/ef-changetracker-entries-where-not-recognized
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;

  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions options)
      : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Article> Articles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()
            .Where(e => typeof(AbstractEntity).IsAssignableFrom(e.ClrType)))
        {
            modelBuilder
                .Entity(entityType.ClrType)
                .Property(nameof(AbstractEntity.UpdatedDate))
                .ValueGeneratedOnAdd()
                .HasDefaultValue(DateTime.UtcNow);
        }

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

    public void dateCreationModification() {
      var now = DateTime.UtcNow;
      var changes = ChangeTracker
        .Entries<AbstractEntity>()
        .Where(e =>
            e.State == EntityState.Added
            || e.State == EntityState.Modified);

      foreach (var item in changes)
      {
        item.Property(p => p.UpdatedDate).CurrentValue = now;
        System.Console.WriteLine("coucou");
        System.Console.WriteLine(item.Property(p => p.UpdatedDate).CurrentValue);

        if (item.State == EntityState.Added)
        {
          item.Property(p => p.CreatedDate).CurrentValue = now;
        }
      }
    }

    public override int SaveChanges()
    {
      this.dateCreationModification();
      return base.SaveChanges();
    }

    // https://stackoverflow.com/questions/26001151/override-savechangesasync
    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default(CancellationToken))
    {
      System.Console.WriteLine("coucou");
      this.dateCreationModification();
      return (await base.SaveChangesAsync(true, cancellationToken));
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
