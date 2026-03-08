using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.UseCases.AccessLevel.Commands.CreateOrUpdateAccessLevel;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class AccessLevelWebService
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/accesslevel";

        public AccessLevelWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResponse<int>> CreateOrUpdateAsync(CreateOrUpdateAccessLevelRequest accessLevel)
        {
            var result = await _http.PostAsJsonAsync(_baseEndpoint, accessLevel);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<int>>();

            return json!;
        }

        public async Task<ApiResponse<IEnumerable<AccessLevelModel>>> GetAllAsync()
        {
            var result = await _http.GetAsync(_baseEndpoint);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<AccessLevelModel>>>();

            return json!;
        }

        public async Task<ApiResponse<AccessLevelModel>> GetById(int accessLevelId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{accessLevelId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<AccessLevelModel>>();

            return json!;
        }
    }
}
