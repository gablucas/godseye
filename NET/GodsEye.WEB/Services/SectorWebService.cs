
using GodsEye.API.DTO;
using GodsEye.Shared.Response.Sector;
using GodsEye.WEB.Model.Forms;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class SectorWebService
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/sector";

        public SectorWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<int> CreateAsync(CreateSectorForm sector)
        {
            var result = await _http.PostAsJsonAsync(_baseEndpoint, sector);

            var json = await result.Content.ReadFromJsonAsync<int>();

            return json!;
        }

        public async Task<SectorResponse> GetById(int sectorId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{sectorId}");

            var json = await result.Content.ReadFromJsonAsync<SectorResponse>();

            return json!;
        }

        public async Task<IEnumerable<SectorResponse>> GetAllAsync()
        {
            var result = await _http.GetAsync(_baseEndpoint);

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<SectorResponse>>();

            return json!;
        }
    }
}
