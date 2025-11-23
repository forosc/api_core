using Application.Models.ImageManagement;

namespace Application.Contracts.Infrastructure
{
    public interface ImanageImageService
    {
        Task<ImageResponde> UploadImage(ImageData imageStream);
        Task DeleteImageAsync(string imageUrl);
    }
}
