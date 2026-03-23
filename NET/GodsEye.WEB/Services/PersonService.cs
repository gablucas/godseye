using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.WEB.Model.Forms;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class PersonService
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/person";

        public PersonService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResponse<ProcedureResult?>> CreateAsync(PersonForm person)
        {
            var result = await _http.PostAsJsonAsync($"{_baseEndpoint}", person);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<ProcedureResult?>>();

            return json!;
        }

        public async Task<ApiResponse<ProcedureResult?>> UpdateAsync(PersonForm person)
        {
            var result = await _http.PutAsJsonAsync($"{_baseEndpoint}", person);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<ProcedureResult?>>();

            return json!;
        }

        public async Task<ApiResponse<ProcedureResult?>> CreateRecognizeAsync(PersonRecognizeForm person)
        {
            var result = await _http.PostAsJsonAsync($"{_baseEndpoint}/recognize", person);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<ProcedureResult>>();

            return json;
        }

        public async Task<ApiResponse<IEnumerable<PersonModel>>> GetAllAsync()
        {
            var result = await _http.GetAsync($"{_baseEndpoint}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<PersonModel>>>();

            return json!;
        }

        public async Task<ApiResponse<PersonModel>> GetById(int personId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{personId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<PersonModel>>();

            return json!;
        }
    }
}
