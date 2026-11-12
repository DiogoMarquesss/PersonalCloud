using Microsoft.EntityFrameworkCore;
using PersonalCloud.Files.Api.Services.Interfaces;
using PersonalCloud.Files.Data.Entities;
using PersonalCloud.Files.Data.Context;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace PersonalCloud.Files.Api.Services;

public class FileService : IFileService
{
    private readonly ILocalFileStorage  _localFileStorage;
    private readonly FileDbContext _context;

    public FileService(ILocalFileStorage localFileStorage, FileDbContext context)
    {
        _localFileStorage = localFileStorage;
        _context = context;
    }

    public async  Task UploadFileAsync(List<IFormFile> files, string userId)
    {
        foreach (var file in files)
        {
            await using var stream = file.OpenReadStream();

            var storedName = Guid.NewGuid().ToString();

            var path = await _localFileStorage.SaveFileDiskAsync(
                stream,
                userId,
                storedName
            );
             
            var newFile = new FileItem
            {
              Name = file.FileName,
              StoragePath = path,
              StoredName = storedName,
              UserId = userId  
            };

            _context.Files.Add(newFile);

            await _context.SaveChangesAsync();
        }
    }

    public async Task<(Stream? Stream, FileItem? File)> GetFileAsync(Guid fileId,string userId)
    {
        var file = await _context.Files
            .FirstOrDefaultAsync(f =>
                f.Id == fileId &&
                f.UserId == userId);

        if (file == null)
            return (null, null);

        var stream = _localFileStorage.GetFileFromDisk(file.StoragePath);

        return (stream, file);
    }

    public async Task<bool> DeleteFile (string userId,Guid fileId)
    {
        var file = await _context.Files
            .FirstOrDefaultAsync(f =>
                f.Id == fileId &&
                f.UserId == userId);

        if (file == null)
            return false;
        
        var isDeleted = _localFileStorage.DeleteFileFromDisk(file.StoragePath);

        if (!isDeleted)
        {
            return false;
        }

        _context.Files.Remove(file);
        
        await _context.SaveChangesAsync();

        return true;
    }
}