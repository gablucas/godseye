using GodsEye.Shared.Response;
using GodsEye.Shared.Response.Person;
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

        public async Task<ProcedureResponse?> CreateAsync(PersonForm person)
        {
            var result = await _http.PostAsJsonAsync($"{_baseEndpoint}", person);

            var json = await result.Content.ReadFromJsonAsync<ProcedureResponse?>();

            return json!;
        }

        public async Task<ProcedureResponse?> UpdateAsync(PersonForm person)
        {
            var result = await _http.PutAsJsonAsync($"{_baseEndpoint}", person);

            var json = await result.Content.ReadFromJsonAsync<ProcedureResponse?>();

            return json!;
        }

        public async Task<ProcedureResponse?> CreateRecognizeAsync(PersonRecognizeForm person)
        {
            var result = await _http.PostAsJsonAsync($"{_baseEndpoint}/recognize", person);

            var json = await result.Content.ReadFromJsonAsync<ProcedureResponse>();

            return json;
        }

        public async Task<IEnumerable<PersonResponse>> GetAllAsync()
        {
            var result = await _http.GetAsync(_baseEndpoint);

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<PersonResponse>>();

            return json!;
        }

        public async Task<PersonResponse> GetById(int personId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{personId}");

            var json = await result.Content.ReadFromJsonAsync<PersonResponse>();

            return json!;
        }
    }
}
