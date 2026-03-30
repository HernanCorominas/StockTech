using Microsoft.AspNetCore.Hosting;
using StockTech.Application.Interfaces;

namespace StockTech.Infrastructure.Services;

public class LocalStorageService : IStorageService
{
    private readonly string _storagePath;

    public LocalStorageService(IWebHostEnvironment env)
    {
        _storagePath = Path.Combine(env.ContentRootPath, "wwwroot", "storage");
        if (!Directory.Exists(_storagePath))
            Directory.CreateDirectory(_storagePath);
    }

    public async Task<string> UploadFileAsync(byte[] content, string fileName, string contentType, string bucketName = "documents")
    {
        var bucketPath = Path.Combine(_storagePath, bucketName);
        if (!Directory.Exists(bucketPath))
            Directory.CreateDirectory(bucketPath);

        var filePath = Path.Combine(bucketPath, fileName);
        await File.WriteAllBytesAsync(filePath, content);

        // Retornamos una ruta relativa o URL ficticia para el desarrollo
        return $"/storage/{bucketName}/{fileName}";
    }

    public async Task<byte[]> DownloadFileAsync(string fileUrl)
    {
        var filePath = Path.Combine(_storagePath, "..", fileUrl.TrimStart('/'));
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Archivo no encontrado en almacenamiento local.");

        return await File.ReadAllBytesAsync(filePath);
    }

    public Task DeleteFileAsync(string fileUrl)
    {
        var filePath = Path.Combine(_storagePath, "..", fileUrl.TrimStart('/'));
        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }
}
