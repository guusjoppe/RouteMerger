using RouteMerger.Persistence.Entities;

namespace RouteMerger.Persistence.Data;

public class RouteMergerDbContext : DbContext
{
    public RouteMergerDbContext(DbContextOptions<RouteMergerDbContext> options) : base(options)
    {
    }

    public DbSet<Route> Routes => Set<Route>();
    public DbSet<FileReference> FileReferences => Set<FileReference>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Route>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<FileReference>()
            .HasKey(fr => fr.Id);

        modelBuilder.Entity<Route>()
            .Property(r => r.Name)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Route>()
            .HasMany<FileReference>(r => r.FileReferences)
            .WithOne()
            .HasForeignKey(fr => fr.RouteId);

        modelBuilder.Entity<FileReference>()
            .Property(fr => fr.FileName)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<FileReference>()
            .Property(fr => fr.RelativePath)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<FileReference>()
            .Property(fr => fr.FileExtension)
            .HasMaxLength(10)
            .IsRequired();
    }
}