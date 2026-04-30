
using GodsEye.API.DTO;
using GodsEye.API.DTOs.Response;
using GodsEye.Shared.Response.Camera;
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

        public async Task<CameraPreviewResponse> StartStream(CameraResponse camera)
        {
            var result = await _http.PostAsJsonAsync($"{_baseEndpoint}/start-stream", camera);

            var json = await result.Content.ReadFromJsonAsync<CameraPreviewResponse>();

            return json!;
        }
    }
}
