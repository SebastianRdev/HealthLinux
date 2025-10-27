using Microsoft.AspNetCore.Mvc;
using ShiftManager.Web.Services;
using ShiftManager.Web.Data.Entities;
using ShiftManager.Web.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ShiftManager.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAffiliateService _affiliateService;

    public AccountController(IAffiliateService affiliateService)
    {
        _affiliateService = affiliateService;
    }

    // Pantalla inicial (login)
    [HttpGet]
    public IActionResult Login() => View();
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _affiliateService.ValidateCredentialsAsync(email, password);
        if (user == null)
        {
            ViewBag.Error = "Invalid credentials"; // Mostrar error si las credenciales son incorrectas
            return View();
        }

        // Crear identidad y firmar al usuario (autenticación por cookie)
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim("Role", user.Role.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // Iniciar sesión
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        // Redirigir a la página principal o dashboard
        TempData["SuccessMessage"] = "Welcome back, " + user.Email + "!";
        return RedirectToAction("Request", "Shift");
    }


    // Redirige a registro
    [HttpPost]
    public async Task<IActionResult> Register(Affiliate affiliate)
    {
        if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(affiliate);
            }
    
    
        var result = await _affiliateService.AddAffiliateAsync(affiliate);

        if (!result)
        {
            ViewBag.Error = "The email is already registered.";
            return View(affiliate);
        }

        // Redirige al index cuando el registro es exitoso
        TempData["SuccessMessage"] = "Account created successfully! You can now access your dashboard.";
        return RedirectToAction("Index", "Home");
    }

    // Google login redirigido por AddGoogleAuthentication
    [HttpGet]
    public IActionResult GoogleLogin(string returnUrl = "/")
    {
        var redirectUrl = Url.Action("GoogleResponse", "Account", new { returnUrl });
        var properties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, "Google");
    }
    
    [HttpGet]
    public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    
        if (result?.Principal == null)
            return RedirectToAction("Login");
    
        var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var fullName = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var names = (fullName ?? "").Split(' ', 2); // divide en máximo 2 partes
        var firstName = names.Length > 0 ? names[0] : "";
        var lastName = names.Length > 1 ? names[1] : "";
    
        if (string.IsNullOrEmpty(email))
            return RedirectToAction("Login");
    
        // Buscar si ya existe
        var user = await _affiliateService.GetAffiliateByEmailAsync(email);
    
        if (user == null)
        {
            // Si no existe, creamos uno temporal y lo enviamos a completar perfil
            user = new Affiliate
            {
                Email = email,
                Name = firstName,
                LastName = lastName,
                Role = 0,
                Password = null
            };
    
            // No lo guardamos aún hasta que complete el perfil
            TempData["IncompleteProfile"] = true;
            return View("CompleteProfile", user);
        }
    
        // Si ya existe, iniciamos sesión normal
        var localClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
    
        var identity = new ClaimsIdentity(localClaims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
    
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    
        return Redirect(returnUrl);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteProfile(Affiliate model)
    {
        if (!ModelState.IsValid)
        {
            var errors = string.Join(", ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            Console.WriteLine(errors);
            ViewBag.Error = "Please correct the form.";
            return View(model);
        }
    
        // Guardar usuario en BD
        await _affiliateService.AddAffiliateAsync(model);
    
        // Autenticamos al usuario inmediatamente después
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, model.Email),
            new Claim(ClaimTypes.Role, model.Role.ToString())
        };
    
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
    
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
    
        TempData["SuccessMessage"] = "Welcome, your profile has been created!";
        return RedirectToAction("Index", "Home");
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // Cerrar sesión
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Redirigir al login después de cerrar sesión
        return RedirectToAction("Login", "Account");
    }

}