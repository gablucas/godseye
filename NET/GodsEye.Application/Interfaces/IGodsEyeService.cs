using GodsEye.Application.DTOs.External.GodsEye;
using GodsEye.Application.DTOs.Response;

namespace GodsEye.Application.Interfaces
{
    public interface IGodsEyeService
    {
        Task<float[]> GenerateEmbedding(byte[] image);
        Task<CameraPreviewResponse> StartStream(string name, string url);
    }
}
