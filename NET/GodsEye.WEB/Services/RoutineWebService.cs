using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.UseCases.Routine.Commands.CreateRoutine;
using GodsEye.Application.UseCases.Routine.Queries;
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

        public async Task<ApiResponse<IEnumerable<RoutineModel>>> GetAllAsync()
        {
            var result = await _http.GetAsync(_baseEndpoint);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<RoutineModel>>>();

            return json!;
        }

        public async Task<RoutineModel> GetById(int routineId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{routineId}");

            var json = await result.Content.ReadFromJsonAsync<RoutineModel>();

            return json!;
        }
    }
}
