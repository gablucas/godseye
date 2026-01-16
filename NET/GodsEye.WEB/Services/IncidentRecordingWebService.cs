using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class IncidentRecordingWebService
    {
        private readonly HttpClient _http;

        public IncidentRecordingWebService(HttpClient http)
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
