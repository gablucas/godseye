using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class EnvironmentMonitoringService
    {
        private readonly HttpClient _http;

        public EnvironmentMonitoringService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResponse<IEnumerable<EnvironmentMonitoringModel>>> GetAllLogs()
        {
            var result = await _http.GetAsync($"api/environmentmonitoring");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<EnvironmentMonitoringModel>>>();

            return json!;
        }
    }
}
