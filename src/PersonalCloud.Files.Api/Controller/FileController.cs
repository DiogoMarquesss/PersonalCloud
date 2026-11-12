using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PersonalCloud.Files.Api.Services.Interfaces;
using PersonalCloud.Common.Security;
using Microsoft.AspNetCore.StaticFiles;

namespace PersonalCloud.Files.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public FilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files)
    {
        if (files == null || files.Count == 0)
            return BadRequest("No files were provided.");

        var userId = User.GetUserId();

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await _fileService.UploadFileAsync(files, userId);

        return Ok("Files successfully uploaded.");
    }

    [HttpGet("{id}/content")]
    public async Task<IActionResult> GetFileContent(Guid id)
    {
        var userId = User.GetUserId();

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _fileService.GetFileAsync(id, userId);

        if (result.File == null || result.Stream == null)
            return NotFound();

        var provider = new FileExtensionContentTypeProvider();

        if (!provider.TryGetContentType(result.File.Name, out var contentType))
            contentType = "application/octet-stream";

        return File(result.Stream, contentType);
    }

    [HttpDelete("{id}/content")]
    public async Task<IActionResult> DeleteFiles (Guid id)
    {
        var userId = User.GetUserId();
        
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _fileService.DeleteFile(userId,id);

        if (!result)
        {
            throw new IOException("Failed to delete file from disk.");
        }

        return Ok("File remove with sucess");
    }
}