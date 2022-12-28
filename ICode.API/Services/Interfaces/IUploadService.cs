using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUploadService
    {
        public Task<string> UploadAsync(ImageUploadParams image);
    }
}
