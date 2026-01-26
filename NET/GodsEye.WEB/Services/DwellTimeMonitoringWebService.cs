using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class DwellTimeMonitoringWebService
    {
        private readonly HttpClient _http;

        public DwellTimeMonitoringWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResponse<IEnumerable<DwellTimeMonitoringDetailsModel>>> GetDetailsByCameraId(int cameraId)
        {
            var result = await _http.GetAsync($"api/dwelltimemonitoring/details/{cameraId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<DwellTimeMonitoringDetailsModel>>>();

            return json!;
        }
    }
}
