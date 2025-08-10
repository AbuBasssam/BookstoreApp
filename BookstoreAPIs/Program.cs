using Application;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Seeder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Presentation;
using PresentationLayer.Middleware;
using Serilog;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Dependency injections

builder.Services
.registerInfrastructureDependencies(builder.Configuration)
.registerApplicationDependencies(builder.Configuration);

#endregion

#region Localization
builder.Services.AddControllersWithViews();
builder.Services.AddLocalization(opt =>
{
    opt.ResourcesPath = "";
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    List<CultureInfo> supportedCultures = new List<CultureInfo>
    {
            new CultureInfo("en"),
            new CultureInfo("ar")
    };

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

#endregion

#region Register Presentation Layer Controllers

var presentationAssembly = typeof(AssemblyReference).Assembly;

builder.Services
    .AddControllers()
    .AddApplicationPart(presentationAssembly);

#endregion

#region Serilog

Log.Logger = new LoggerConfiguration().ReadFrom
      .Configuration(builder.Configuration)
      .CreateLogger();
builder.Services.AddSerilog();

#endregion

var app = builder.Build();

#region Seeder
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
    var DbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await RoleSeeder.SeedAsync(roleManager);
    await UserSeeder.SeedAsync(userManager);
    await SystemSettingsSeeder.SeedAsync(DbContext);
    await BookSeeder.SeedAsync(DbContext);
    await LanguageSeeder.SeedAsync(DbContext);
    //await SPAndFunctionsSeeder.SeedAsync(DbContext);


}

#endregion

#region Localization Middleware
var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(options.Value);
#endregion

#region Middlewares

app.UseMiddleware<GlobalRateLimitingMiddleware>();

app.UseMiddleware<SensitiveRateLimitingMiddleware>();

app.UseMiddleware<ErrorHandlerMiddleware>();

#endregion

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
