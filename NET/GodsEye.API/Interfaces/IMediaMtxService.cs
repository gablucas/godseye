namespace GodsEye.API.Interfaces
{
    public interface IMediaMtxService
    {
        Task<bool> IsOnlineAsync();
        Task<string> StartStream(string url);
        Task<(bool, string)> GetStream(string rtspUrl);
    }
}
