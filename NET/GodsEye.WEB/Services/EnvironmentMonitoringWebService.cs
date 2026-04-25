
using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Shared.Response.EnvironmentMonitoring;
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

        public async Task<IEnumerable<EnvironmentMonitoringLogResponse>> GetAllLogs(int pageNumber, int pageSize)
        {
            var result = await _http.GetAsync($"api/environmentmonitoring/log?pageNumber={pageNumber}&pageSize={pageSize}");

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<EnvironmentMonitoringLogResponse>>();

            return json!;
        }

        public async Task<IEnumerable<EnvironmentMonitoringLogResponse>> GetLastRegisterPerPerson()
        {
            var result = await _http.GetAsync($"api/environmentmonitoring/log/last-per-person");

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<EnvironmentMonitoringLogResponse>>();

            return json!;
        }

        public async Task<IEnumerable<GetEnviromentMonitoringPerSectorResponse>> GetSectors()
        {
            var result = await _http.GetAsync($"api/environmentmonitoring/sectors");

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<GetEnviromentMonitoringPerSectorResponse>>();

            return json!;
        }

        public async Task<EnvironmentMonitoringPersonResponse> GetByPersonId(int personId)
        {
            var result = await _http.GetAsync($"api/environmentmonitoring/person/{personId}");

            var json = await result.Content.ReadFromJsonAsync<EnvironmentMonitoringPersonResponse>();

            return json!;
        }

        public async Task<bool> DeleteAllLogs()
        {
            var result = await _http.DeleteAsync($"api/environmentmonitoring");
            var json = await result.Content.ReadFromJsonAsync<bool>();
            return json!;
        }
    }
}
