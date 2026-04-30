namespace GodsEye.API.Exceptions
{
    public class MediaMtxServiceException : Exception
    {
        public int StatusCode { get; }

        public MediaMtxServiceException(string message, int statusCode = 500) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
