namespace ShiftManager.Web.Repositories;

using Microsoft.EntityFrameworkCore;
using ShiftManager.Web.Data;
using ShiftManager.Web.Interfaces;
using ShiftManager.Web.Data.Entities;

public class AffiliateRepository : IAffiliateRepository
{
    private readonly ApplicationDbContext _context;

    public AffiliateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Affiliate>> GetAllAsync() =>
        await _context.Affiliates.ToListAsync();

    public async Task<Affiliate?> GetByIdAsync(int id) =>
        await _context.Affiliates.FindAsync(id);
    
    public async Task<Affiliate?> GetByEmailAsync(string email)
    {
        return await _context.Affiliates.FirstOrDefaultAsync(a => a.Email == email);
    }
    
    public async Task<Affiliate?> GetByIdentificationAsync(string identification)
    {
        return await _context.Affiliates.FirstOrDefaultAsync(a => a.Identification == identification);
    }

    public async Task AddAsync(Affiliate affiliate)
    {
        await _context.Affiliates.AddAsync(affiliate);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Affiliate affiliate)
    {
        _context.Affiliates.Update(affiliate);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Affiliates.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}