namespace ShiftManager.Web.Interfaces;

using ShiftManager.Web.Data.Entities;

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
}