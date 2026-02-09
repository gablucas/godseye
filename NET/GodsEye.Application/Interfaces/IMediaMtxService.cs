using GodsEye.Application.DTOs.Response;

namespace GodsEye.Application.Interfaces
{
    public interface IMediaMtxService
    {
        Task<bool> IsOnlineAsync();
        Task<string> StartStream(string url);
        Task<(bool, string)> GetStream(string rtspUrl);
    }
}
