namespace ShiftManager.Web.Services;

using ShiftManager.Web.Interfaces;
using ShiftManager.Web.Data.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;


public class AffiliateService : IAffiliateService
{
    private readonly IAffiliateRepository _repository;
    private readonly GoogleDriveService _googleDriveService;

    public AffiliateService(IAffiliateRepository repository, GoogleDriveService googleDriveService)
    {
        _repository = repository;
        _googleDriveService = googleDriveService;
    }
    
    // Login normal
    public async Task<ClaimsPrincipal?> LoginAsync(string email, string password)
    {
        var user = await _repository.GetByEmailAsync(email);
        if (user == null || user.Password != password) // Aquí puedes usar hash si lo implementas
            return null;

        return CreateClaimsPrincipal(user);
    }

    
    // Register normal
    public async Task<(bool Success, ClaimsPrincipal? Principal)> RegisterAsync(Affiliate affiliate, IFormFile? photo)
    {
        var exists = await _repository.GetByEmailAsync(affiliate.Email);
        if (exists != null) return (false, null);

        // Automatic values
        if (string.IsNullOrWhiteSpace(affiliate.UniqueCode))
            affiliate.UniqueCode = Guid.NewGuid().ToString();
        if (string.IsNullOrWhiteSpace(affiliate.MembershipNumber))
            affiliate.MembershipNumber = $"M-{DateTime.UtcNow:yyyyMMddHHmmss}";

        // Upload photo if available
        if (photo != null)
        {
            var photoUrl = await _googleDriveService.UploadPhotoAsync(photo.OpenReadStream(), affiliate.UniqueCode);
            affiliate.PhotoUrl = photoUrl;
        }

        await _repository.AddAsync(affiliate);

        var principal = CreateClaimsPrincipal(affiliate);
        return (true, principal);
    }

    
    // Login with Google
    public async Task<(Affiliate? User, bool IsNew)> GoogleLoginAsync(string email, string firstName, string lastName)
    {
        var user = await _repository.GetByEmailAsync(email);
        if (user != null)
            return (user, false);

        // Create temporary user to complete profile
        user = new Affiliate
        {
            Email = email,
            Name = firstName,
            LastName = lastName,
            Role = 0
        };

        return (user, true);
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
    
    // Complete profile
    public async Task<ClaimsPrincipal?> CompleteProfileAsync(Affiliate model)
    {
        var existing = await _repository.GetByEmailAsync(model.Email);
        if (existing == null) return null;

        existing.Identification = model.Identification;
        existing.Phone = model.Phone;
        existing.Address = model.Address;
        existing.BirthDate = model.BirthDate;

        await _repository.UpdateAsync(existing);
        return CreateClaimsPrincipal(existing, profileComplete: true);
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
    
    // Helper
    private ClaimsPrincipal CreateClaimsPrincipal(Affiliate user, bool profileComplete = false)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        if (profileComplete)
            claims.Add(new Claim("ProfileComplete", "True"));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        return new ClaimsPrincipal(identity);
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
                Console.WriteLine($"[WARN] Duplicate email: {affiliate.Email}");
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
            Console.WriteLine($"[OK] Updated affiliate: {affiliate.Email}");
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
            Console.WriteLine($"[OK] Deleted affiliate: {id}");
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
                Console.WriteLine($"[WARN] Email not found: {email}");
                return null;
            }

            if (affiliate.Password != password)
            {
                Console.WriteLine($"[WARN] Incorrect password for {email}");
                return null;
            }

            Console.WriteLine($"[OK] Login successful: {email}");
            return affiliate;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] ValidateCredentialsAsync: {ex.Message}");
            return null;
        }
    }
    
    public async Task<(ClaimsPrincipal? Principal, bool IsNew, Affiliate? User)> ProcessGoogleLoginAsync(HttpContext context)
    {
        var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var claims = result.Principal?.Identities.FirstOrDefault()?.Claims;

        var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var fullName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";

        if (string.IsNullOrEmpty(email))
            return (null, false, null);

        var names = fullName.Split(' ', 2);
        var firstName = names.ElementAtOrDefault(0);
        var lastName = names.ElementAtOrDefault(1);

        var (user, isNew) = await GoogleLoginAsync(email, firstName!, lastName!);

        if (!isNew && user != null)
        {
            var principal = CreateClaimsPrincipal(user, profileComplete: true);
            return (principal, false, null);
        }

        return (null, isNew, user);
    }

}
