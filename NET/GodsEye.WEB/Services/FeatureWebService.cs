using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class FeatureWebService
    {
        private readonly HttpClient _http;

        public FeatureWebService(HttpClient http) { 
            _http = http;
        }

        public async Task<ApiResponse<IReadOnlyCollection<FeatureModel>>> GetAllAsync()
        {
            var result = await _http.GetAsync("api/feature");
            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IReadOnlyCollection<FeatureModel>>>();
            return json!;
        }
    }
}
