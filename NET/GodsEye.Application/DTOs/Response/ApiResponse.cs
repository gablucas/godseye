namespace GodsEye.Application.DTOs.Response
{
    public class ApiResponse<T>
    {
        public bool Sucesso { get; set; }
        public T? Dados { get; set; }
        public ApiErrorResponse? Erro { get; set; }

        public static ApiResponse<T> Ok(T data)
            => new ApiResponse<T> { Sucesso = true, Dados = data };

        public static ApiResponse<T> Fail(int statusCode, string message, string? detail = null)
            => new ApiResponse<T>
            {
                Sucesso = false,
                Erro = new ApiErrorResponse
                {
                    StatusCode = statusCode,
                    Message = message,
                    Detail = detail
                }
            };
    }
}
