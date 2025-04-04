using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using online.shop.api.Data;
using online.shop.api.Models;
using online.shop.api.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Configuration.AddEnvironmentVariables();

// Configure Identity with Entity Framework
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 36)); // Adjust based on your MySQL version

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseMySql(connectionString, serverVersion)
);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.SignIn.RequireConfirmedAccount = false; // Adjust if you want confirmed accounts for login
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<WishlistService>();
builder.Services.AddSingleton<KeyVaultService>();

// Configure Serilog
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

Log.Information("Application is starting...");

// Seed users and roles
using (var scope = app.Services.CreateScope())
{
    try
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); // Ensures DB is created

        await DataSeeder.SeedUsersAndRolesAsync(services);
        await DataSeeder.SeedProductsAsync(context);
    }
    catch (Exception e)
    {
        Log.Error(e, "Database Error...");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Use session middleware
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Enable Identity default routes
app.MapRazorPages(); // <-- This enables login, register, and other Identity pages.

app.MapControllerRoute(name: "default", pattern: "{controller=Products}/{action=Index}/{id?}");

app.Run();
