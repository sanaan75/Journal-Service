using Journal_Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace Journal_Service.Data;

public class AppDbContext : DbContext, IDb
{
    public AppDbContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IEntityTypeConfiguration<>).Assembly);
        SetConventions(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    private static void SetConventions(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var foreignKey in entityType.GetForeignKeys())
            {
                foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<Journal> Journals { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=192.168.1.2;Initial Catalog=Banks;uid=sanaan;pwd=Sanaan51!@;TrustServerCertificate=True");
    }

    public void Save()
    {
        SaveChanges();
    }

    public Task<int> SaveAsync()
    {
        throw new NotImplementedException();
    }

    public IQueryable<TEntity> Query<TEntity>() where TEntity : class, IEntity
    {
        return Set<TEntity>().AsQueryable().OrderByDescending(i => i.Id);
    }
}