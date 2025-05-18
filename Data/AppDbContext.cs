using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<UserEntity>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AdoptionApplicationEntity> AdoptionApplications { get; set; }
    public DbSet<PetEntity> Pets { get; set; }
    public DbSet<ShelterEntity> Shelters { get; set; }

    // public DbSet<UserEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserEntity>()
        .ToTable("Users");

        // Configure folder relationships
        builder.Entity<AdoptionApplicationEntity>(entity =>
        {
            entity.HasKey(a => a.Id);

            // Store enum as string in DB
            entity.Property(a => a.AdoptionStatus)
                .HasConversion<string>();

            entity.HasOne(a => a.User)
                .WithMany(u => u.AdoptionApplications)
                .HasForeignKey(a => a.UserId);

            entity.HasOne(a => a.Pet)
                .WithMany(p => p.AdoptionApplications)
                .HasForeignKey(a => a.PetId);
        });

        builder.Entity<PetEntity>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Gender)
                .HasConversion<string>();

            entity.Property(p => p.Species)
                .HasConversion<string>();

            entity.HasOne(p => p.Shelter)
                .WithMany(s => s.Pets)
                .HasForeignKey(p => p.ShelterId);

            entity.HasMany(p => p.AdoptionApplications)
                .WithOne(a => a.Pet)
                .HasForeignKey(a => a.PetId);
        });

        builder.Entity<ShelterEntity>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.HasOne(s => s.User)
                .WithOne(u => u.Shelter)
                .HasForeignKey<ShelterEntity>(s => s.UserId);
        });

        builder.Entity<UserEntity>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.HasOne(u => u.Shelter)
                .WithOne(s => s.User)
                .HasForeignKey<ShelterEntity>(s => s.UserId);

            entity.HasMany(u => u.AdoptionApplications)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId);
        });
    }
}






