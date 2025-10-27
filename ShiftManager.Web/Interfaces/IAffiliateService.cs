namespace ShiftManager.Web.Interfaces;

using ShiftManager.Web.Data.Entities;
using System.Security.Claims;

public interface IAffiliateService
{
    Task<IEnumerable<Affiliate>> GetAllAffiliatesAsync();
    Task<Affiliate?> GetAffiliateByIdAsync(int id);
    Task<Affiliate?> GetAffiliateByEmailAsync(string email);
    Task<Affiliate?> GetAffiliateByIdentificationAsync(string identification);
    Task<bool> AddAffiliateAsync(Affiliate affiliate);
    Task UpdateAffiliateAsync(Affiliate affiliate);
    Task DeleteAffiliateAsync(int id);
    Task<Affiliate?> ValidateCredentialsAsync(string email, string password);
    Task<ClaimsPrincipal?> LoginAsync(string email, string password);
    Task<(bool Success, ClaimsPrincipal? Principal)> RegisterAsync(Affiliate affiliate, IFormFile? photo);
    Task<(Affiliate? User, bool IsNew)> GoogleLoginAsync(string email, string firstName, string lastName);
    Task<ClaimsPrincipal?> CompleteProfileAsync(Affiliate model);
    Task<(ClaimsPrincipal? Principal, bool IsNew, Affiliate? User)> ProcessGoogleLoginAsync(HttpContext context);
}