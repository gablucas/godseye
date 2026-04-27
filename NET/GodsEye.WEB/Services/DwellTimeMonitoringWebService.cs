using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class DwellTimeMonitoringWebService
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/dwell-time-monitoring";

        public DwellTimeMonitoringWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<DwellTimeMonitoringDetailsModel>> GetDetailsByCameraId(int cameraId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/details/{cameraId}");

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<DwellTimeMonitoringDetailsModel>>();

            return json!;
        }
    }
}
