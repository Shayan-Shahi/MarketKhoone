using Microsoft.AspNetCore.Http;

namespace MarketKhoone.Services.Contracts
{
    public interface IUploadedFileService
    {
        Task SaveFile(IFormFile file, string fileName, string oldFileName, params string[] destinationDirectoryNames);
        void DeleteFile(string fileName, params string[] destinationDirectoryNames);
    }
}
