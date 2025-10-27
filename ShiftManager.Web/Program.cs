using Microsoft.EntityFrameworkCore;
using ShiftManager.Web.Data;
using ShiftManager.Web.Interfaces;
using ShiftManager.Web.Services;
using ShiftManager.Web.Extensions;
using ShiftManager.Web.Repositories;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// Configure main services
builder.Services
    .AddDatabase(builder.Configuration)         // Database configuration
    .AddGoogleAuthentication()                  // Google authentication settings
    .AddControllersWithViews();                 // Classic MVC (controllers + views)
    

builder.Services
    .AddScoped<IAffiliateRepository, AffiliateRepository>()
    .AddScoped<IAffiliateService, AffiliateService>();

builder.Services.AddHttpContextAccessor();   // To access HttpContext from views
builder.Services.AddSingleton(new GoogleDriveService("/home/Cohorte3/Música/ShiftManager.Web/credentials.json"));


// Application Development
var app = builder.Build();


// Middleware Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔐
app.UseAuthentication();
app.UseAuthorization();


// Main Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// DB Connection Test (only in development)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        db.Database.OpenConnection();
        Console.WriteLine("✅ Successful connection to the database");
        db.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error connecting to the database:");
        Console.WriteLine(ex.Message);
    }
}

app.Run();
