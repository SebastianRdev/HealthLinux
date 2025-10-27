namespace ShiftManager.Web.Extensions;

using Microsoft.EntityFrameworkCore;
using ShiftManager.Web.Data;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST");
        var port = Environment.GetEnvironmentVariable("DB_PORT");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var pass = Environment.GetEnvironmentVariable("DB_PASS");
        var name = Environment.GetEnvironmentVariable("DB_NAME");

        var connectionString = $"Server={host};Port={port};Database={name};User={user};Password={pass};";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        );

        return services;
    }
}