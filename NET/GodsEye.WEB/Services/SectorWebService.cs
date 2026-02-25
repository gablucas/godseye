using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
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

        public async Task<ApiResponse<int>> CreateAsync(CreateSectorForm sector)
        {
            var result = await _http.PostAsJsonAsync(_baseEndpoint, sector);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<int>>();

            return json!;
        }

        public async Task<ApiResponse<SectorModel>> GetById(int sectorId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{sectorId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<SectorModel>>();

            return json!;
        }

        public async Task<ApiResponse<IEnumerable<SectorModel>>> GetAllAsync()
        {
            var result = await _http.GetAsync(_baseEndpoint);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<SectorModel>>>();

            return json!;
        }
    }
}
