﻿using Application.Behaviors;
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
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
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

        HomePageSetting(services, configuration);

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
        services.AddScoped<IStringLocalizer<SharedResources>, StringLocalizer<SharedResources>>();

        services.AddSwaggerGen(_GetSecurityRequirement);




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

        services.AddScoped<ISessionTokenService, SessionTokenService>();



    }

    private static void _AddPolicies(this IServiceCollection services)
    {
        // add policies for authorization
        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.VerificationOnly, policy =>
                policy.Requirements.Add(new VerificationOnlyRequirement()));

            options.AddPolicy(Policies.ResetPasswordOnly, policy =>
                policy.Requirements.Add(new ResetPasswordOnlyRequirement()));
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
    private static void HomePageSetting(IServiceCollection services, IConfiguration configuration)
    {
        var homePageSetting = configuration.GetSection("HomePageSettings");
        services.Configure<HomePageSettings>(homePageSetting);

        var Setting = new HomePageSettings();

        configuration.GetSection(nameof(HomePageSettings)).Bind(Setting);

        services.AddSingleton(Setting);
    }

    private static void _getMediatRServiceConfiguration(MediatRServiceConfiguration config)
        => config.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly());

    private static void FluentValidatorConfiguration(IServiceCollection services)
    {
        //Configuration for FluentValidator

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }
    private static void SwaggerGenInfo(SwaggerGenOptions option)
    {
        option.SwaggerDoc("v1", new OpenApiInfo { Title = "BookstoreApp", Version = "v1" });
        option.EnableAnnotations();

        option.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer schema (e.g. Bearer your token)",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
        });
    }
    private static void _GetSecurityRequirement(SwaggerGenOptions option)
    {
        var openApiReference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }; // Should match the scheme name
        var securityScheme = new OpenApiSecurityScheme { Reference = openApiReference, Name = JwtBearerDefaults.AuthenticationScheme, In = ParameterLocation.Header };
        var securityRequirement = new OpenApiSecurityRequirement { { securityScheme, new List<string>() } };
        SwaggerGenInfo(option);
        option.AddSecurityRequirement(securityRequirement);// This is a list of scopes, which is empty in this case

    }
}
