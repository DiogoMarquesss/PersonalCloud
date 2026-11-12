using PersonalCloud.Files.Data.Entities;

namespace PersonalCloud.Files.Api.Services.Interfaces;

public interface IFileService
{
    Task UploadFileAsync(List<IFormFile> files, string userId);
    Task<(Stream? Stream, FileItem? File)> GetFileAsync(Guid fileId,string userId);
    Task<bool> DeleteFile (string userId,Guid fileId);

    
}