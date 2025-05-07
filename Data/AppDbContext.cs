using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<PetEntity> Pets { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<ShelterEntity> Shelters { get; set; }
    public DbSet<AdoptionApplicationEntity> AdoptionApplictions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PetEntity>(entity =>
        {

        });



    }


}






