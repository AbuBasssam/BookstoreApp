using Application.Behaviors;
using Application.Interfaces;
using Application.Models;
using Application.Services;
using ApplicationLayer.Resources;
using Domain.AppMetaData;
using Domain.HelperClasses;
using Domain.Security;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;


namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection registerApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
    {

        JWTAuthentication(services, configuration);

        ServicesRegisteration(services);

        EmailSetting(services, configuration);

        FluentValidatorConfiguration(services);

        // add authorization policies
        _AddPolicies(services);

        // Configuration for MediaR
        services.AddMediatR(_getMediatRServiceConfiguration);

        //Configuration for AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        services.AddTransient<ResponseHandler>();


        //Localization
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        services.AddScoped<IStringLocalizer<SharedResoruces>, StringLocalizer<SharedResoruces>>();

        return services;
    }

    private static void JWTAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        //JWT Authentication
        var jwtSection = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(jwtSection);


        var JwtSettings = new JwtSettings();


        configuration.GetSection(nameof(JwtSettings)).Bind(JwtSettings);

        services.AddSingleton(JwtSettings);

        services
            .AddAuthentication(_authenticationInfo)
            .AddJwtBearer(option => { _JwtBearerInfo(option, JwtSettings); });
    }

    private static void _JwtBearerInfo(JwtBearerOptions option, JwtSettings JwtSettings)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = JwtSettings.ValidateIssuer,
            ValidIssuers = new[] { JwtSettings.Issuer },
            ValidateAudience = JwtSettings.ValidateAudience,
            ValidAudience = JwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.Secret!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        option.RequireHttpsMetadata = false;
        option.SaveToken = true;

        option.TokenValidationParameters = validationParameters;
    }

    private static void _authenticationInfo(AuthenticationOptions option)
    {
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }

    private static void ServicesRegisteration(IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IEmailsService, EmailsService>();



    }

    private static void _AddPolicies(this IServiceCollection services)
    {
        // add policies for authorization
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.VerificationOnly, policy =>
                policy.Requirements.Add(new VerificationOnlyRequirement()));
        });

    }

    private static void EmailSetting(IServiceCollection services, IConfiguration configuration)
    {
        //way 1: Using IOptions<T>
        /*
        var EmailSetting = configuration.GetSection("emailSettings");

        services.Configure<EmailSettings>(EmailSetting);

        services.AddSingleton(EmailSetting);
        */

        // way 2: Using Bind method
        var emailSetting = configuration.GetSection("emailSettings");
        services.Configure<EmailSettings>(emailSetting);

        var EmailSetting = new EmailSettings();

        configuration.GetSection(nameof(EmailSettings)).Bind(EmailSetting);

        services.AddSingleton(EmailSetting);
    }

    private static void _getMediatRServiceConfiguration(MediatRServiceConfiguration config)
        => config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());

    private static void FluentValidatorConfiguration(IServiceCollection services)
    {
        //Configuration for FluentValidator

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }
}
