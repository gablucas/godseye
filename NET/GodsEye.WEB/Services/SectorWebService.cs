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

        public SectorWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResponse<ProcedureResult?>> CreateAsync(CreateSectorForm sector)
        {
            var result = await _http.PostAsJsonAsync("api/sector", sector);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<ProcedureResult?>>();

            return json!;
        }

        public async Task<ApiResponse<IEnumerable<SectorModel>>> GetAllAsync()
        {
            var result = await _http.GetAsync("api/sector");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<SectorModel>>>();

            return json!;
        }
    }
}
