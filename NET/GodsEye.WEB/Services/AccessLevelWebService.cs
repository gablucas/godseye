using GodsEye.Shared.Response.AccessLevel;
using GodsEye.WEB.Model.Forms;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class AccessLevelWebService
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/access-level";

        public AccessLevelWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<int> CreateOrUpdateAsync(AccessLevelForm accessLevel)
        {
            var result = await _http.PostAsJsonAsync(_baseEndpoint, accessLevel);

            var json = await result.Content.ReadFromJsonAsync<int>();

            return json!;
        }

        public async Task<IEnumerable<AccessLevelResponse>> GetAllAsync()
        {
            var result = await _http.GetAsync(_baseEndpoint);

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<AccessLevelResponse>>();

            return json!;
        }

        public async Task<AccessLevelResponse> GetById(int accessLevelId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{accessLevelId}");

            var json = await result.Content.ReadFromJsonAsync<AccessLevelResponse>();

            return json!;
        }
    }
}
