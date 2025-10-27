namespace ShiftManager.Web.Helpers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using ShiftManager.Web.Data.Entities;

public static class ClaimsHelper
{
    public static ClaimsPrincipal CreateAffiliatePrincipal(Affiliate user, bool profileComplete = false)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        if (profileComplete)
            claims.Add(new Claim("ProfileComplete", "True"));
            
        if (!string.IsNullOrEmpty(user.PhotoUrl))
            claims.Add(new Claim("PhotoPublicId", user.PhotoUrl));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}