using Domain.HelperClasses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BookstoreApplication;

public static class DependencyInjection
{
    public static IServiceCollection registerApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        //FluentValidatorConfiguration(services);

        JWTAuthentication(services, configuration);

        //GoogleAuthentication(services, configuration);

        //TwilioSetting(services, configuration);

        //StripeSetting(services, configuration);

        //EmailSetting(services, configuration);


        //ServicesRegisteration(services);

        //services.AddSwaggerGen(_GetSecurityRequirement);

        //Configuration for AutoMapper
        //services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Configuration for MediaR
        //services.AddMediatR(_getMediatRServiceConfiguration);

        //services.AddTransient<ResponseHandler>();

        //services.AddLocalization(options => options.ResourcesPath = "Resources");


        //services.AddTransient<IStringLocalizer<SharedResoruces>, StringLocalizer<SharedResoruces>>();



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

    private static void _authenticationInfo(Microsoft.AspNetCore.Authentication.AuthenticationOptions option)
    {
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
}
