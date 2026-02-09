using GodsEye.Application.DTOs.Response;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class MediaMtxWebService
    {
        private readonly HttpClient _http;
        private readonly string _baseEnpoint = "api/mediamtx";

        public MediaMtxWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResponse<bool>> CheckStatus()
        {
            var response = await _http.GetAsync($"{_baseEnpoint}/status");
            var json = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
            return json!;
        }

        public async Task<ApiResponse<string>> StartStream(string rtspUrl)
        {
            var payload = new { RtspUrl = rtspUrl };

            var response = await _http.PostAsJsonAsync($"{_baseEnpoint}/start-stream", payload);

            var json = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();

            return json!;
        }
    }
}
