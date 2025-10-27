namespace ShiftManager.Web.Services;

using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(string cloudName, string apiKey, string apiSecret)
    {
        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
        Console.WriteLine("✅ Cloudinary initialized");
    }

    // Subir foto como privada
    public async Task<string> UploadPrivatePhotoAsync(Stream fileStream, string affiliateId)
    {
        Console.WriteLine($"📤 Uploading photo for: {affiliateId}");

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription($"{affiliateId}.jpg", fileStream),
            PublicId = $"affiliates/{affiliateId}",
            Type = "private", // ⭐ Privada, no accesible sin token
            Overwrite = true,
            Transformation = new Transformation()
                .Width(800)
                .Height(800)
                .Crop("limit")
                .Quality("auto")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
        {
            Console.WriteLine($"❌ Upload failed: {result.Error.Message}");
            throw new Exception($"Cloudinary upload failed: {result.Error.Message}");
        }

        Console.WriteLine($"✅ Photo uploaded: {result.PublicId}");
        return result.PublicId; // Retorna: "affiliates/abc-123"
    }

    // Generar URL firmada temporal (expira en 1 hora por defecto)
    public string GetSecureUrl(string publicId, int expiresInSeconds = 3600)
    {
        if (string.IsNullOrEmpty(publicId))
            return null;

        var timestamp = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds).ToUnixTimeSeconds();

        var url = _cloudinary.Api.UrlImgUp
            .Secure(true)
            .PrivateCdn(false)
            .Type("private")
            .Signed(true)
            .Transform(new Transformation()
                .Width(500)
                .Height(500)
                .Crop("limit")
                .Quality("auto"))
            .BuildUrl(publicId);

        Console.WriteLine($"🔗 Generated secure URL: {url}");
        return url;
    }

    // Eliminar foto
    public async Task<bool> DeletePhotoAsync(string publicId)
    {
        if (string.IsNullOrEmpty(publicId))
            return false;

        var deleteParams = new DeletionParams(publicId)
        {
            Type = "private"
        };

        var result = await _cloudinary.DestroyAsync(deleteParams);
        
        Console.WriteLine($"🗑️ Photo deleted: {publicId}");
        return result.Result == "ok";
    }
}