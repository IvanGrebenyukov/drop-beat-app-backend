using Microsoft.AspNetCore.Http;

namespace DropBeatAPI.Core.Interfaces;

public interface IYandexStorageService
{
    Task<Stream?> GetFileAsync(string fileKey);
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder);
    Task<bool> DeleteFileAsync(string fileKey);
    Task<string> UploadFileAsync(IFormFile file, string fileName, string contentType, string folder);
    Task<IEnumerable<string>> ListFilesAsync(string prefix);
}