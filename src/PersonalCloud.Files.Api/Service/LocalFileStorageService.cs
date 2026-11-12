using PersonalCloud.Files.Api.Services.Interfaces;

namespace PersonalCloud.Files.Api.Services;

public class LocalFileStorageService : ILocalFileStorage
{
    public async Task<string> SaveFileDiskAsync(Stream stream, string userId, string fileName)
    {
        var basePath = Environment.GetEnvironmentVariable("STORAGE_BASE_PATH");

        if (string.IsNullOrWhiteSpace(basePath))
            throw new InvalidOperationException("Storage path not configured.");

        var userPath = Path.Combine(basePath, userId);

        Directory.CreateDirectory(userPath);

        var filePath = Path.Combine(userPath, fileName);

        await using var fileStream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write
        );

        await stream.CopyToAsync(fileStream);

        return Path.Combine(userId, fileName);
    }

    public Stream? GetFileFromDisk(string relativePath)
    {
        var basePath = Environment.GetEnvironmentVariable("STORAGE_BASE_PATH");

        if (string.IsNullOrWhiteSpace(basePath))
            throw new InvalidOperationException("Storage path not configured.");

        var filePath = Path.Combine(basePath, relativePath);

        if (!File.Exists(filePath))
            return null;

        return File.OpenRead(filePath);
    }
}