namespace Videotheque.Data
{
  using System;
  // https://stackoverflow.com/questions/36798186/ef-changetracker-entries-where-not-recognized
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.EntityFrameworkCore;

  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions options)
      : base(options)
    {
    }

    internal DbSet<Article> Articles { get; set; }

    internal DbSet<Customer> Customers { get; set; }

    internal DbSet<Film> Films { get; set; }

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

        // modelBuilder.Entity<Article>()
        //     .HasAlternateKey(a => a.Barcode)
        //     .HasName("AlternateKey_BareCode");
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

    private void DateCreationModification()
    {
      var now = DateTime.UtcNow;
      var changes = base.ChangeTracker
        .Entries<AbstractEntity>()
        .Where(e =>
            e.State == EntityState.Added
            || e.State == EntityState.Modified);

      foreach (var item in changes)
      {
        item.Property(p => p.UpdatedDate).CurrentValue = now;
        System.Console.WriteLine(item.Property(p => p.UpdatedDate).CurrentValue);

        if (item.State == EntityState.Added)
        {
          item.Property(p => p.CreatedDate).CurrentValue = now;
        }
      }
    }

    public override int SaveChanges()
    {
      this.DateCreationModification();
      return base.SaveChanges();
    }

    // https://stackoverflow.com/questions/26001151/override-savechangesasync
    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default(CancellationToken))
    {
      this.DateCreationModification();
      return await base.SaveChangesAsync(true, cancellationToken).
        ConfigureAwait(false);
    }

  }
}

// vim:sw=2:ts=2:et:fileformat=dos
