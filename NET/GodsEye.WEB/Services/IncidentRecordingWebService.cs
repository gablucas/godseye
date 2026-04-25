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

        public async Task<ApiResponse<IEnumerable<IncidentRecordingModel>>> GetAllLogs(int pageNumber, int pageSize)
        {
            var result = await _http.GetAsync($"api/incident-recording?pageNumber={pageNumber}&pageSize={pageSize}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<IncidentRecordingModel>>>();

            return json!;
        }
    }
}
