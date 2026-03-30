namespace StockTech.Application.Interfaces;

public interface IStorageService
{
    Task<string> UploadFileAsync(byte[] content, string fileName, string contentType, string bucketName = "documents");
    Task<byte[]> DownloadFileAsync(string fileUrl);
    Task DeleteFileAsync(string fileUrl);
}
