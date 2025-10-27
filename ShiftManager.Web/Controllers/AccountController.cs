using Microsoft.AspNetCore.Mvc;
using ShiftManager.Web.Services;
using ShiftManager.Web.Data.Entities;
using ShiftManager.Web.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ShiftManager.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAffiliateService _affiliateService;
    private readonly CloudinaryService _cloudinaryService;

    public AccountController(IAffiliateService affiliateService, CloudinaryService cloudinaryService)
    {
        _affiliateService = affiliateService;
        _cloudinaryService = cloudinaryService;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var principal = await _affiliateService.LoginAsync(email, password);
        if (principal == null)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return View();
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        TempData["SuccessMessage"] = "Welcome back!";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Register(Affiliate affiliate, IFormFile? Photo)
    {
        if (!ModelState.IsValid) return View(affiliate);

        var (success, principal) = await _affiliateService.RegisterAsync(affiliate, Photo);

        if (!success)
        {
            ModelState.AddModelError("", "The email is already registered.");
            return View(affiliate);
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal!);
        TempData["SuccessMessage"] = "Account created successfully!";
        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    public IActionResult GoogleLogin(string returnUrl = "/")
    {
        var redirectUrl = Url.Action("GoogleResponse", "Account", new { returnUrl });
        var properties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
        {
            RedirectUri = redirectUrl,
            Items =
            {
                // Force account selector
                { "prompt", "select_account" }
            }
        };
        return Challenge(properties, "Google");
    }

    [HttpGet]
    public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
    {
        var (principal, isNew, user) = await _affiliateService.ProcessGoogleLoginAsync(HttpContext);
        
        if (principal != null)
        {
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return Redirect(returnUrl);
        }

        if (isNew && user != null)
            return View("CompleteProfile", user);

        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> CompleteProfile(Affiliate model)
    {
        if (!ModelState.IsValid) 
            return View(model);

        var principal = await _affiliateService.CompleteProfileAsync(model);
        if (principal == null)
        {
            ModelState.AddModelError("", "User not found.");
            return View(model);
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        TempData["SuccessMessage"] = "Welcome! Your profile has been completed.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
}
