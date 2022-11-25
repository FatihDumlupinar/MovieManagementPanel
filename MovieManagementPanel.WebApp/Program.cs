using Microsoft.AspNetCore.Authentication.Cookies;
using MovieManagementPanel.Persistence.Extensions;
using MovieManagementPanel.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

#region Appsettings

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

var configuration = new ConfigurationBuilder()
.SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
.AddJsonFile($"appsettings.{env}.json", optional: true)
.AddEnvironmentVariables();

var config = configuration.Build();

if (config == null)
{
    throw new ArgumentNullException(nameof(configuration));
}

#endregion

builder.Services.AddPersistenceServices(config);

builder.Services.AddControllersWithViews();

var mvcBuilder = builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x =>
{
    x.LoginPath = "/Account/Login";
    x.AccessDeniedPath = "/Errors/AccessDenied";
    x.LogoutPath = "/Account/Logout";

});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Errors/error-development");
}
else
{
    app.UseExceptionHandler("/Errors/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.Services.SeedAsync();

app.Run();
