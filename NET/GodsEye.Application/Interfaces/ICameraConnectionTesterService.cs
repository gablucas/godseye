namespace GodsEye.Application.Interfaces
{
    public interface ICameraConnectionTesterService
    {
        Task<(bool IsSuccess, string Message)> TestConnectionAsync(string rtspUrl);
    }
}
