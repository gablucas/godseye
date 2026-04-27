using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class GodsEyeWebService
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/godseye";

        public GodsEyeWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<CameraPreviewResponse> StartStream(CameraModel camera)
        {
            var result = await _http.PostAsJsonAsync($"{_baseEndpoint}/start-stream", camera);

            var json = await result.Content.ReadFromJsonAsync<CameraPreviewResponse>();

            return json!;
        }
    }
}
