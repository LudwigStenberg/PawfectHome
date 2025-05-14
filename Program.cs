using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace PawfectHome;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        // builder.Services.AddAuthentication()
        // .AddBearerToken(IdentityConstants.BearerScheme);
        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();

        builder
            .Services.AddIdentityApiEndpoints<UserEntity>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        );

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

        var app = builder.Build();

        app.MapOpenApi();
        // app.MapScalarApiEndpoints();

        // Configure the HTTP request pipeline.
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
