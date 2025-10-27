namespace ShiftManager.Web.Helpers;

using System.Security.Claims;
using ShiftManager.Web.Services;

public static class LayoutHelper
{
    public static string GetUserPhotoUrl(ClaimsPrincipal user, CloudinaryService cloudinaryService)
    {
        var photoPublicId = user.FindFirst("PhotoPublicId")?.Value;
            
        if (!string.IsNullOrEmpty(photoPublicId))
            return cloudinaryService.GetSecureUrl(photoPublicId);
            
        return "https://cdn-icons-png.flaticon.com/512/847/847969.png";
    }
}