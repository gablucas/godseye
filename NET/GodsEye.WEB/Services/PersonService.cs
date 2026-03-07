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

        public PersonService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResponse<ProcedureResult?>> CreateAsync(CreatePersonForm person)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(person.Name), "Name");
            content.Add(new StringContent(person.Photo), "Photo");
            content.Add(new StringContent(person.Sector.ToString()), "Sector");

            var result = await _http.PostAsync("api/person", content);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<ProcedureResult?>>();

            return json!;
        }

        public async Task<ApiResponse<IEnumerable<PersonModel>>> GetAllAsync()
        {
            var result = await _http.GetAsync("api/person");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<PersonModel>>>();

            return json!;
        }

        public async Task<ApiResponse<PersonModel>> GetById(int personId)
        {
            var result = await _http.GetAsync($"api/person/{personId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<PersonModel>>();

            return json!;
        }
    }
}
