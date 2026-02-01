using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.WEB.Model.Forms;
using MudBlazor;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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

            foreach (var sector in person.Sectors)
            {
                content.Add(new StringContent(sector.ToString()), "Sectors");
            }

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

        public async Task<ApiResponse<IEnumerable<PersonLogModel>>> GetLogs(int personId)
        {
            var result = await _http.GetAsync($"api/person/logs/{personId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<PersonLogModel>>>();

            return json!;
        }

        public async Task<ApiResponse<IEnumerable<PersonLogModel>>> GetAllLogs()
        {
            var result = await _http.GetAsync($"api/person/logs");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<PersonLogModel>>>();

            return json!;
        }
    }
}
