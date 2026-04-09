using GodsEye.Application.DTOs.Response;

namespace GodsEye.Application.Interfaces
{
    public interface IGodsEyeService
    {
        Task<float[]> GenerateEmbedding(byte[] image);
        Task ProcessingEmbedding(int CameraId, float[] Embedding, DateTime ExtractedAt);
        Task<CameraPreviewResponse> StartStream(string name, string url);
    }
}
