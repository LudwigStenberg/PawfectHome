
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace PawfectHome;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<IShelterRepository, ShelterRepository>();
        builder.Services.AddScoped<IShelterService, ShelterService>();
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();

        builder.Services.AddIdentityApiEndpoints<UserEntity>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


        var app = builder.Build();

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
