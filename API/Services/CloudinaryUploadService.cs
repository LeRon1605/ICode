using API.Models.DTO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace API.Services
{
    public class CloudinaryUploadService : IUploadService
    {
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinary;
        public CloudinaryUploadService(IConfiguration configuration)
        {
            _configuration = configuration;
            CloudinarySetting cloudinarySetting = _configuration.GetSection("Cloudinary").Get<CloudinarySetting>();
            _cloudinary = new Cloudinary(new Account
            {
                Cloud = cloudinarySetting.CloudName,
                ApiKey = cloudinarySetting.ClientID,
                ApiSecret = cloudinarySetting.SecretKey
            });
        }
        public async Task<string> UploadAsync(ImageUploadParams image)
        {
            var result = await _cloudinary.UploadAsync(image);
            if (result.Error != null)
            {
                return null;
            }
            else
            {
                return result.Url.ToString();
            }
        }
    }
}
