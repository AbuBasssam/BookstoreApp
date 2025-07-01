using Application.Interfaces;
using Domain.Entities;
using Implementations;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        // Register the custom authorization handlers
        AddHandlers(services);


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
         "abcdefghijklmnopqrstuvwxyz" +
         "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
         "0123456789";


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

        services.AddScoped<IOtpRepsitory, OtpRepository>();


    }
    private static void AddHandlers(IServiceCollection services)
    {
        // Add your custom authorization handlers here
        services.AddScoped<IAuthorizationHandler, VerificationOnlyHandler>();
    }
}
