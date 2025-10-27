namespace ShiftManager.Web.Interfaces;

using ShiftManager.Web.Data.Entities;

public interface IAffiliateRepository
{
    Task<IEnumerable<Affiliate>> GetAllAsync();
    Task<Affiliate?> GetByIdAsync(int id);
    Task<Affiliate?> GetByEmailAsync(string email);
    Task<Affiliate?> GetByIdentificationAsync(string identification);
    Task AddAsync(Affiliate affiliate);
    Task UpdateAsync(Affiliate affiliate);
    Task DeleteAsync(int id);
}