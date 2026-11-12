using PersonalCloud.Files.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace PersonalCloud.Files.Data.Context;
public class FileDbContext : DbContext
{
    public FileDbContext(DbContextOptions<FileDbContext> options) : base(options)
    {
    }

    public DbSet<FileItem> Files { get; set; } = null!;
}