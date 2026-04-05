using GodsEye.Application.DTOs.Response;
using GodsEye.Application.UseCases.Routine.Commands.CreateRoutine;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class RoutineWebService
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/routine";

        public RoutineWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResponse<int>> CreateAsync(CreateRoutineRequest routine)
        {
            var result = await _http.PostAsJsonAsync(_baseEndpoint, routine);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<int>>();

            return json!;
        }
    }
}
