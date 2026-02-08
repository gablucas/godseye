namespace GodsEye.Application.Interfaces
{
    public interface IMediaMtxService
    {
        Task<string> StartStream(string url);
    }
}
