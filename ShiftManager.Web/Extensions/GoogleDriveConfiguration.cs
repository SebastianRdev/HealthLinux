namespace ShiftManager.Web.Extensions;

using ShiftManager.Web.Services;

using Microsoft.Extensions.DependencyInjection;

public static class GoogleDriveConfiguration
{
    public static IServiceCollection AddGoogleDriveService(this IServiceCollection services)
    {
        services.AddSingleton<GoogleDriveService>();
        return services;
    }
}