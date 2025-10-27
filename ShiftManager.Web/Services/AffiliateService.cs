namespace ShiftManager.Web.Services;

using ShiftManager.Web.Interfaces;
using ShiftManager.Web.Data.Entities;

public class AffiliateService : IAffiliateService
{
    private readonly IAffiliateRepository _repository;

    public AffiliateService(IAffiliateRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Affiliate>> GetAllAffiliatesAsync()
    {
        try
        {
            return await _repository.GetAllAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] GetAllAffiliatesAsync: {ex.Message}");
            return Enumerable.Empty<Affiliate>();
        }
    }

    public async Task<Affiliate?> GetAffiliateByIdAsync(int id)
    {
        try
        {
            return await _repository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] GetAffiliateByIdAsync: {ex.Message}");
            return null;
        }
    }
    
    public async Task<Affiliate?> GetAffiliateByEmailAsync(string email)
    {
        return await _repository.GetByEmailAsync(email);
    }
    
    public async Task<Affiliate?> GetAffiliateByIdentificationAsync(string identification)
    {
        return await _repository.GetByIdentificationAsync(identification);
    }


    public async Task<bool> AddAffiliateAsync(Affiliate affiliate)
    {
        try
        {
            Console.WriteLine($" Adding new affiliate");
            var existing = await _repository.GetByEmailAsync(affiliate.Email);
            if (existing != null)
            {
                Console.WriteLine($"[WARN] Email duplicado: {affiliate.Email}");
                return false;
            }

            // Generate automatic values
            if (string.IsNullOrWhiteSpace(affiliate.UniqueCode))
                affiliate.UniqueCode = Guid.NewGuid().ToString();

            if (string.IsNullOrWhiteSpace(affiliate.MembershipNumber))
                affiliate.MembershipNumber = $"M-{DateTime.UtcNow:yyyyMMddHHmmss}";

            await _repository.AddAsync(affiliate);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] AddAffiliateAsync: {ex.Message}");
            return false;
        }
    }

    public async Task UpdateAffiliateAsync(Affiliate affiliate)
    {
        try
        {
            await _repository.UpdateAsync(affiliate);
            Console.WriteLine($"[OK] Afiliado actualizado: {affiliate.Email}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] UpdateAffiliateAsync: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteAffiliateAsync(int id)
    {
        try
        {
            await _repository.DeleteAsync(id);
            Console.WriteLine($"[OK] Afiliado eliminado: {id}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] DeleteAffiliateAsync: {ex.Message}");
            throw;
        }
    }

    public async Task<Affiliate?> ValidateCredentialsAsync(string email, string password)
    {
        try
        {
            var affiliate = await _repository.GetByEmailAsync(email);
            if (affiliate == null)
            {
                Console.WriteLine($"[WARN] Email no encontrado: {email}");
                return null;
            }

            if (affiliate.Password != password)
            {
                Console.WriteLine($"[WARN] Contraseña incorrecta para {email}");
                return null;
            }

            Console.WriteLine($"[OK] Inicio de sesión correcto: {email}");
            return affiliate;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] ValidateCredentialsAsync: {ex.Message}");
            return null;
        }
    }
}
