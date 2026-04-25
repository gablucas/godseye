using GodsEye.Shared.Response.AccessSchedule;
using GodsEye.WEB.Model.Forms;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class AccessScheduleWebService
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/access-schedule";

        public AccessScheduleWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<int> CreateAsync(AccessScheduleForm accessSchedule)
        {
            var result = await _http.PostAsJsonAsync(_baseEndpoint, accessSchedule);

            var json = await result.Content.ReadFromJsonAsync<int>();

            return json!;
        }

        public async Task<IEnumerable<AccessScheduleResponse>> GetAllAsync()
        {
            var result = await _http.GetAsync(_baseEndpoint);

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<AccessScheduleResponse>>();

            return json!;
        }

        public async Task<AccessScheduleResponse> GetById(int accessScheduleId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{accessScheduleId}");

            var json = await result.Content.ReadFromJsonAsync<AccessScheduleResponse>();

            return json!;
        }
    }
}
