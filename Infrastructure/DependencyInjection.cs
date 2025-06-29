using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection registerInfrastructureDependencies(this IServiceCollection services, IConfiguration configuration)
    {

        DbContextRegisteration(services, configuration);

        IdentityRegisteration(services);

        RepsitoriesRegisteration(services);


        return services;
    }
    private static void DbContextRegisteration(IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<AppDbContext>(options =>
        options
        .UseLazyLoadingProxies()
        .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
        );
    }

    private static void IdentityRegisteration(IServiceCollection services)
    {
        services.AddIdentityCore<User>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedAccount = true;
            options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+\r\n\r\n";

        })
        .AddUserManager<UserManager<User>>()
        .AddRoles<Role>()
        .AddRoleManager<RoleManager<Role>>()
        .AddEntityFrameworkStores<AppDbContext>();
    }
    private static void RepsitoriesRegisteration(IServiceCollection services)
    {
        // Register the repository and Unit of Work
        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

    }
}
