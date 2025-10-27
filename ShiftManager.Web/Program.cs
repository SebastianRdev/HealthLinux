using Microsoft.EntityFrameworkCore;
using ShiftManager.Web.Data;
using ShiftManager.Web.Interfaces;
using ShiftManager.Web.Services;
using ShiftManager.Web.Extensions;
using ShiftManager.Web.Repositories;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// CONFIGURAR SERVICIOS PRINCIPALES
builder.Services
    .AddDatabase(builder.Configuration)         // Configuración de la base de datos
    .AddGoogleAuthentication()                  // Configuración de autenticación Google
    .AddControllersWithViews();                 // MVC clásico (controladores + vistas)

builder.Services
    .AddScoped<IAffiliateRepository, AffiliateRepository>()
    .AddScoped<IAffiliateService, AffiliateService>();

builder.Services.AddHttpContextAccessor();   // Para acceder a HttpContext desde las vistas

// CONSTRUCCIÓN DE LA APLICACIÓN
var app = builder.Build();


// PIPELINE DE MIDDLEWARE
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


// RUTEO PRINCIPAL
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

/*
app.MapControllerRoute(
    name: "login",
    pattern: "login",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");*/



// 🧪 PRUEBA DE CONEXIÓN A BD (solo en desarrollo)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    try
    {
        db.Database.OpenConnection();
        Console.WriteLine("✅ Conexión exitosa a la base de datos.");
        db.Database.CloseConnection();
    }
    catch (Exception ex)
    {
        Console.WriteLine("❌ Error al conectar con la base de datos:");
        Console.WriteLine(ex.Message);
    }
}

app.Run();
