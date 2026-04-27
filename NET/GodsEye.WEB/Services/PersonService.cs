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

        public async Task<ProcedureResult?> CreateAsync(PersonForm person)
        {
            var result = await _http.PostAsJsonAsync($"{_baseEndpoint}", person);

            var json = await result.Content.ReadFromJsonAsync<ProcedureResult?>();

            return json!;
        }

        public async Task<ProcedureResult?> UpdateAsync(PersonForm person)
        {
            var result = await _http.PutAsJsonAsync($"{_baseEndpoint}", person);

            var json = await result.Content.ReadFromJsonAsync<ProcedureResult?>();

            return json!;
        }

        public async Task<ProcedureResult?> CreateRecognizeAsync(PersonRecognizeForm person)
        {
            var result = await _http.PostAsJsonAsync($"{_baseEndpoint}/recognize", person);

            var json = await result.Content.ReadFromJsonAsync<ProcedureResult>();

            return json;
        }

        public async Task<IEnumerable<PersonModel>> GetAllAsync()
        {
            var result = await _http.GetAsync(_baseEndpoint);

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<PersonModel>>();

            return json!;
        }

        public async Task<PersonModel> GetById(int personId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{personId}");

            var json = await result.Content.ReadFromJsonAsync<PersonModel>();

            return json!;
        }
    }
}
