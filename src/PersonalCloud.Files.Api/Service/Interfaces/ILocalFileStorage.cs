namespace PersonalCloud.Files.Api.Services.Interfaces;

public interface ILocalFileStorage
{
    Task<string> SaveFileDiskAsync(Stream stream, string userId, string fileName);
    Stream? GetFileFromDisk(string relativePath);

    bool DeleteFileFromDisk(string relativePath);
}