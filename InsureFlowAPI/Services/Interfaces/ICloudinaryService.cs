using Microsoft.AspNetCore.Http;

namespace InsureFlowAPI.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}