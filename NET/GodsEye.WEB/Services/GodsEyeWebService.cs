using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class GodsEyeWebService
    {
        private readonly HttpClient _http;

        public GodsEyeWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResponse<CameraPreviewResponse>> StartStream(CameraModel camera)
        {
            var result = await _http.PostAsJsonAsync("api/godseye/start-stream", camera);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<CameraPreviewResponse>>();

            return json!;
        }
    }
}
