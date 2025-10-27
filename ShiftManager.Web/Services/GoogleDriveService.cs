using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using GFile = Google.Apis.Drive.v3.Data.File;

public class GoogleDriveService
{
    private readonly DriveService _driveService;
    private readonly string _photosFolderName = "AffiliatePhotos";
    private string? _photosFolderId;

    public GoogleDriveService(string credentialsPath)
    {
        var credential = GoogleCredential.FromFile(credentialsPath)
                                         .CreateScoped(DriveService.Scope.Drive);

        _driveService = new DriveService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "ShiftManager"
        });
    }

    // 1️⃣ Obtener o crear la carpeta de fotos
    public async Task<string> GetOrCreatePhotosFolderAsync()
    {
        if (_photosFolderId != null)
            return _photosFolderId;

        // Buscar carpeta existente
        var listRequest = _driveService.Files.List();
        listRequest.Q = $"mimeType='application/vnd.google-apps.folder' and name='{_photosFolderName}'";
        listRequest.Fields = "files(id, name)";
        var result = await listRequest.ExecuteAsync();

        if (result.Files?.Count > 0)
        {
            _photosFolderId = result.Files[0].Id;
            return _photosFolderId;
        }

        // Crear carpeta si no existe
        var folderMetadata = new GFile
        {
            Name = _photosFolderName,
            MimeType = "application/vnd.google-apps.folder"
        };

        var createRequest = _driveService.Files.Create(folderMetadata);
        createRequest.Fields = "id";
        var folder = await createRequest.ExecuteAsync();

        _photosFolderId = folder.Id;
        Console.WriteLine($"✅ Photos folder created: ID={_photosFolderId}");
        return _photosFolderId!;

    }

    // 2️⃣ Subir foto
    public async Task<string> UploadPhotoAsync(Stream fileStream, string affiliateId)
    {
        var folderId = await GetOrCreatePhotosFolderAsync();

        var fileMetadata = new GFile
        {
            Name = $"{affiliateId}.jpg",
            Parents = new List<string> { folderId },
            MimeType = "image/jpeg"
        };

        var request = _driveService.Files.Create(fileMetadata, fileStream, "image/jpeg");
        request.Fields = "id, name";
        await request.UploadAsync();

        var file = request.ResponseBody;
        if (file == null || string.IsNullOrEmpty(file.Id))
            throw new Exception("Upload failed: no file ID returned");

        Console.WriteLine($"✅ Photo uploaded: ID={file.Id}, Name={file.Name}");
        return $"/Affiliate/Photo/{affiliateId}";
    }

}
