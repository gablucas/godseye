using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class FeatureWebService
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/feature";

        public FeatureWebService(HttpClient http) { 
            _http = http;
        }

        public async Task<IReadOnlyCollection<FeatureModel>> GetAllAsync()
        {
            var result = await _http.GetAsync(_baseEndpoint);
            var json = await result.Content.ReadFromJsonAsync<IReadOnlyCollection<FeatureModel>>();
            return json!;
        }
    }
}
