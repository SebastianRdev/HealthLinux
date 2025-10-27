namespace ShiftManager.Web.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

public static class AuthenticationConfiguration
{
    public static IServiceCollection AddGoogleAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(options =>
            {
                options.ClientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
                options.ClientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
                options.CallbackPath = "/signin-google";
            });

        return services;
    }
}