using Entities;
using Entities.Conferences;
using Entities.Journals;
using Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence;

public class AppDbContext : DbContext, IDb
{
    private readonly IConfiguration _configuration;

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(_configuration.GetConnectionString("MainConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(_PersistenceDummy).Assembly);
        
        SetConventions(modelBuilder);
        SeedUser(modelBuilder);
        
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
    public DbSet<UserGroup> UserGroups { get; set; }
    public DbSet<UserInGroup> UserInGroups { get; set; }
    public DbSet<UserGroupPermission> UserGroupPermissions { get; set; }
    public DbSet<Journal> Journals { get; set; }
    public DbSet<JournalRecord> JournalRecords { get; set; }
    public DbSet<Conference> Conferences { get; set; }
    
    private void SeedUser(ModelBuilder builder)
    {
        // builder.Entity<User>().HasData(
        //     new User
        //     {
        //         Id = 1,
        //         Email = "sanaanmarabi@gmail.com",
        //         Password = HashPassword.Hash("sanaanmarabi@gmail.com", "@dminB@nk1401!@"),
        //         Type = UserType.SuperAdmin,
        //         Name = "jiro",
        //         Enabled = true,
        //         ConfirmCode = "24121",
        //         ConfirmCodeDate = DateTime.Now
        //     });
    }

    public virtual bool AutoDetectChanges
    {
        get => ChangeTracker.AutoDetectChangesEnabled;
        set => ChangeTracker.AutoDetectChangesEnabled = value;
    }

    public int? CommandTimeOut
    {
        set => Database.SetCommandTimeout(value);
    }

    public void AddPostSaveAction(Action action)
    {
        throw new NotImplementedException();
    }

    public void RemoveById<TEntity>(int id) where TEntity : class, IEntity, new()
    {
        throw new NotImplementedException();
    }

    public void RemoveByIds<TEntity>(IEnumerable<int> ids) where TEntity : class, IEntity, new()
    {
        throw new NotImplementedException();
    }

    public void Save()
    {
        SaveChanges();
    }

    public IQueryable<TEntity> Query<TEntity>() where TEntity : class, IEntity
    {
        return Set<TEntity>().AsQueryable().OrderByDescending(i => i.Id);
    }
}