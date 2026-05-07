using GodsEye.Shared.Response.EnvironmentMonitoring;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class EnvironmentMonitoringWebService
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/environment-monitoring";

        public EnvironmentMonitoringWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<EnvironmentMonitoringResponse>> GetAllLogs(int pageNumber, int pageSize)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/log?pageNumber={pageNumber}&pageSize={pageSize}");

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<EnvironmentMonitoringResponse>>();

            return json!;
        }

        public async Task<IEnumerable<EnvironmentMonitoringResponse>> GetLastRegisterPerPerson()
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/log/last-per-person");

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<EnvironmentMonitoringResponse>>();

            return json!;
        }

        public async Task<IEnumerable<GetEnviromentMonitoringPerSectorResponse>> GetSectors()
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/sectors");

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<GetEnviromentMonitoringPerSectorResponse>>();

            return json!;
        }

        public async Task<EnvironmentMonitoringPersonResponse> GetByPersonId(int personId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/person/{personId}");

            var json = await result.Content.ReadFromJsonAsync<EnvironmentMonitoringPersonResponse>();

            return json!;
        }

        public async Task<bool> DeleteAllLogs()
        {
            var result = await _http.DeleteAsync(_baseEndpoint);
            var json = await result.Content.ReadFromJsonAsync<bool>();
            return json!;
        }
    }
}
