using Microsoft.AspNetCore.Mvc;
using ShiftManager.Web.Services;
using ShiftManager.Web.Interfaces;
using ShiftManager.Web.Models;

namespace ShiftManager.Web.Controllers;

public class ShiftController : Controller
{
    private readonly IAffiliateService _affiliateService;
    
    public ShiftController(IAffiliateService affiliateService)
    {
        _affiliateService = affiliateService;
    }
    
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
    // Página para ingresar identificación y pedir turno
    [HttpGet]
    public IActionResult Request() => View();

    [HttpPost]
    public async Task<IActionResult> Request(string identification)
    {
        var user = await _affiliateService.GetAffiliateByIdentificationAsync(identification);

        if (user == null)
        {
            // No afiliado → mensaje + botón para registro
            ViewBag.Message = "You are not registered with the system. Would you like to register?";
            TempData["ErrorMessage"] = "No estás afiliado. Por favor verifica tu identificación.";
            return RedirectToAction("Request");
        }

        // Simulación de creación de turno
        /*
        var turno = new Random().Next(100, 999);
        ViewBag.Turno = turno;
        ViewBag.Nombre = user.Name;
        */

        return View("Display");
    }

    // Página estilo TV
    public IActionResult Display() => View();
}