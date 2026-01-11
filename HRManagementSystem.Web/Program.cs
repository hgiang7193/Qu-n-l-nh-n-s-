using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using HRManagementSystem.Web.Data;
using HRManagementSystem.Web.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); // This includes both MVC and API controllers

// Add TempData service required for MVC
builder.Services.AddSession();

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found."), 
        new MySqlServerVersion(new Version(8, 0, 21))));

// Add authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "Cookies";
})
.AddCookie("Cookies", options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
});

// Add authorization
builder.Services.AddAuthorization();

// Register services
builder.Services.AddScoped<DatabaseInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Map legacy /Roles route to /Role
app.MapGet("/Roles", () => Results.Redirect("/Role"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Ensure database schema is up-to-date
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    // Apply pending migrations to ensure all tables exist
    try
    {
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        // If migration fails, try EnsureCreated as fallback
        Console.WriteLine($"Migration failed: {ex.Message}, attempting EnsureCreated");
        context.Database.EnsureCreated();
    }
    
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        await initializer.InitializeAsync();
        logger.LogInformation("Database initialized successfully with seed data.");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Database initialization failed. Application will run with limited functionality until database is available.");
        // Continue anyway - the app can run but with limited functionality
    }
}

app.Run();