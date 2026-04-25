using GodsEye.API.DTO;

namespace GodsEye.API.Interfaces
{
    public interface IGodsEyeService
    {
        Task<float[]> GenerateEmbedding(byte[] image);
        Task ProcessingEmbedding(int CameraId, float[] Embedding, DateTime ExtractedAt);
        Task<CameraPreviewResponse> StartStream(string name, string url);
    }
}
