using Microsoft.AspNetCore.Mvc;
using ShiftManager.Web.Data.Entities;
using ShiftManager.Web.Interfaces;

namespace ShiftManager.Web.Controllers;

public class AffiliateController : Controller
{
    private readonly IAffiliateService _affiliateService;

    public AffiliateController(IAffiliateService affiliateService)
    {
        _affiliateService = affiliateService;
    }
    
    
}