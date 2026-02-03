using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class EnvironmentMonitoringWebService
    {
        private readonly HttpClient _http;

        public EnvironmentMonitoringWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResponse<IEnumerable<EnvironmentMonitoringModel>>> GetAllLogs(int pageNumber, int pageSize)
        {
            var result = await _http.GetAsync($"api/environmentmonitoring?pageNumber={pageNumber}&pageSize={pageSize}");

            result.EnsureSuccessStatusCode();

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<EnvironmentMonitoringModel>>>();

            return json!;
        }

        public async Task<ApiResponse<IEnumerable<EnvironmentMonitoringModel>>> GetLastRegisterPerPerson()
        {
            var result = await _http.GetAsync($"api/environmentmonitoring/last-register-per-person");

            result.EnsureSuccessStatusCode();

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<EnvironmentMonitoringModel>>>();

            return json!;
        }

        public async Task<ApiResponse<IEnumerable<EnvironmentMonitoringSectorModel>>> GetSectors()
        {
            var result = await _http.GetAsync($"api/environmentmonitoring/sectors");

            result.EnsureSuccessStatusCode();

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<EnvironmentMonitoringSectorModel>>>();

            return json!;
        }

        public async Task<ApiResponse<EnvironmentMonitoringPersonModel>> GetByPersonId(int personId)
        {
            var result = await _http.GetAsync($"api/environmentmonitoring/person/{personId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<EnvironmentMonitoringPersonModel>>();

            return json!;
        }
    }
}
