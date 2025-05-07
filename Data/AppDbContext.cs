using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Shelter> Shelters { get; set; }
    public DbSet<AdoptionApplication> AdoptionApplictions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }


}






