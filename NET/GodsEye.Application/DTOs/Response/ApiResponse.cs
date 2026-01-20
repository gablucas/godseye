namespace GodsEye.Application.DTOs.Response
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public ApiErrorResponse? Error { get; set; }

        public static ApiResponse<T> Ok(T data)
            => new ApiResponse<T> { Success = true, Data = data };

        public static ApiResponse<T> Fail(int statusCode, string message, string? detail = null)
            => new ApiResponse<T>
            {
                Success = false,
                Error = new ApiErrorResponse
                {
                    StatusCode = statusCode,
                    Message = message,
                    Detail = detail
                }
            };
    }
}
