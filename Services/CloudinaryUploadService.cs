using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models.DTO;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Services
{
    public class CloudinaryUploadService : IUploadService
    {
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinary;
        private readonly ILogger _logger;
        public CloudinaryUploadService(IConfiguration configuration, ILoggerFactory factory)
        {
            _configuration = configuration;
            CloudinarySetting cloudinarySetting = _configuration.GetSection("Cloudinary").Get<CloudinarySetting>();
            _cloudinary = new Cloudinary(new Account
            {
                Cloud = cloudinarySetting.CloudName,
                ApiKey = cloudinarySetting.ClientID,
                ApiSecret = cloudinarySetting.SecretKey
            });
            _logger = factory.CreateLogger<CloudinaryUploadService>();
        }
        public async Task<string> UploadAsync(ImageUploadParams image)
        {
            var result = await _cloudinary.UploadAsync(image);
            if (result.Error != null)
            {
                _logger.LogWarning("Upload Error for file: '{File}'{Time}", image.File.FileName, DateTime.UtcNow);
                return null;
            }
            else
            {
                return result.Url.ToString();
            }
        }
    }
}
