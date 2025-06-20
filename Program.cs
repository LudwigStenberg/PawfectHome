using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace PawfectHome;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<IAdoptionService, AdoptionService>();
        builder.Services.AddScoped<IAdoptionRepository, AdoptionRepository>();
        builder.Services.AddScoped<IPetService, PetService>();
        builder.Services.AddScoped<IPetRepository, PetRepository>();
        builder.Services.AddScoped<IShelterRepository, ShelterRepository>();
        builder.Services.AddScoped<IShelterService, ShelterService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ModelValidator>();
        builder.Services.AddLogging();
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("ShelterOwner", policy => policy.RequireRole("ShelterOwner"));
        });

        builder
            .Services.AddIdentityApiEndpoints<UserEntity>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        );

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            using var roleManager = scope.ServiceProvider.GetRequiredService<
                RoleManager<IdentityRole>
            >();

            if (!roleManager.RoleExistsAsync("ShelterOwner").Result)
            {
                var role = new IdentityRole("ShelterOwner");
                var result = roleManager.CreateAsync(role).Result;
            }
        }

        app.MapOpenApi();

        if (app.Environment.IsDevelopment())
        {
            app.MapScalarApiReference();
        }

        app.MapIdentityApi<UserEntity>();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
