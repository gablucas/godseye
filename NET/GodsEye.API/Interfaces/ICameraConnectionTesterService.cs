namespace GodsEye.API.Interfaces
{
    public interface ICameraConnectionTesterService
    {
        Task<(bool IsSuccess, string Message)> TestConnectionAsync(string rtspUrl);
    }
}
