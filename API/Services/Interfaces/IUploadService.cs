using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IUploadService
    {
        public Task<string> UploadAsync(ImageUploadParams image);
    }
}
