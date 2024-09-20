using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public interface IUserHelpers
    {

        Task<string> AddImage(IFormFile file, string folderName);
        Task<bool> DeleteImageAsync(string fileName, string folderName);
        Task<string> UpdateImageAsync(IFormFile? file, string fileName, string folderName);
    }
}
