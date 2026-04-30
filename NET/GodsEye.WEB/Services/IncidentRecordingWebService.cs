using GodsEye.Shared.Response.IncidentRecording;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class IncidentRecordingWebService
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/incident-recording";

        public IncidentRecordingWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<IncidentRecordingResponse>> GetAllLogs(int pageNumber, int pageSize)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}?pageNumber={pageNumber}&pageSize={pageSize}");

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<IncidentRecordingResponse>>();

            return json!;
        }
    }
}
