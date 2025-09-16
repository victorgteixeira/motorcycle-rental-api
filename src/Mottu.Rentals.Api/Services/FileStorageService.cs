using Microsoft.AspNetCore.Http;

namespace Moto.Rentals.Api.Services;

public sealed class FileStorageService(IConfiguration cfg)
{
    private readonly string _root = cfg["Storage:CnhRoot"] ?? "./storage/cnh";

    public async Task<string> SaveCnhAsync(Guid courierId, IFormFile file, CancellationToken ct)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext is not (".png" or ".bmp"))
            throw new InvalidOperationException("Only PNG or BMP allowed");

        Directory.CreateDirectory(_root);
        var filePath = Path.Combine(_root, $"{courierId}{ext}");
        using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await file.CopyToAsync(fs, ct);
        return filePath;
    }
}
