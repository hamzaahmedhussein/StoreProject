#region imports
using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
#endregion


namespace Application.Services
{
    public class UserHelpers : IUserHelpers
    {
        #region Constructor
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<IUserHelpers> _logger;
        private readonly IAccountService _accountService;

        public UserHelpers(IConfiguration config, UserManager<AppUser> userManager, IHttpContextAccessor contextAccessor,
            IWebHostEnvironment webHostEnvironment, ApplicationDbContext context, ILogger<IUserHelpers> logger, IAccountService accountService)
        {
            _config = config;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _logger = logger;
            _accountService = accountService;
        }
        #endregion





        #region Image Management
        public async Task<string> AddImage(IFormFile? file, string profileType)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty.", nameof(file));
            }

            string rootPath = _webHostEnvironment.WebRootPath;
            var user = await _accountService.GetCurrentUserAsync();
            string userName = user.UserName;
            string profileFolderPath = Path.Combine(rootPath, "Images", userName, profileType);

            if (!Directory.Exists(profileFolderPath))
            {
                Directory.CreateDirectory(profileFolderPath);
            }

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(profileFolderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/Images/{userName}/{profileType}/{fileName}";
        }


        public async Task<bool> DeleteImageAsync(string imagePath, string profileType)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                throw new ArgumentException("Image path is null or empty.", nameof(imagePath));
            }

            string rootPath = _webHostEnvironment.WebRootPath;
            var user = await _accountService.GetCurrentUserAsync();
            string userName = user.UserName;

            if (!imagePath.StartsWith($"/Images/{userName}/{profileType}/"))
            {
                throw new ArgumentException("Invalid image path.", nameof(imagePath));
            }

            string filePath = Path.Combine(rootPath, imagePath.TrimStart('/'));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            else
            {
                throw new FileNotFoundException("File not found.", filePath);
            }
        }

        public async Task<string> UpdateImageAsync(IFormFile? file, string oldImagePath, string folderName)
        {
            await DeleteImageAsync(oldImagePath, folderName);
            string newImagePath = await AddImage(file, folderName);
            return newImagePath;
        }
        #endregion

    }
}
