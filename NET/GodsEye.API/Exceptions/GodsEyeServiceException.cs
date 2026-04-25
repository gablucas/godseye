namespace GodsEye.API.Exceptions
{
    public class GodsEyeServiceException : Exception
    {
        public int StatusCode { get; }

        public GodsEyeServiceException(string message, int statusCode = 500) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
