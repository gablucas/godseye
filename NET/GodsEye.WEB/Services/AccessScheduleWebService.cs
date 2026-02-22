using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.UseCases.AccessSchedule.Commands.CreateAccessSchedule;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class AccessScheduleWebService
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/accessschedule";

        public AccessScheduleWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateAccessScheduleRequest accessSchedule)
        {
            var result = await _http.PostAsJsonAsync(_baseEndpoint, accessSchedule);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<int>>();

            return json!;
        }

        public async Task<ApiResponse<IEnumerable<AccessScheduleModel>>> GetAllAsync()
        {
            var result = await _http.GetAsync(_baseEndpoint);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<AccessScheduleModel>>>();

            return json!;
        }

        public async Task<ApiResponse<AccessScheduleModel>> GetById(int accessScheduleId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{accessScheduleId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<AccessScheduleModel>>();

            return json!;
        }
    }
}
