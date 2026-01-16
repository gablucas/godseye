using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.WEB.Model.Forms;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class CameraService
    {
        private readonly HttpClient _http;

        public CameraService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResponse<ProcedureResult?>> CreateAsync(CreateCameraForm camera)
        {
            var result = await _http.PostAsJsonAsync("api/camera", camera);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<ProcedureResult?>>();

            return json!;
        }

        public async Task<ApiResponse<IEnumerable<CameraModel>>> GetAllAsync()
        {
            var result = await _http.GetAsync("api/camera");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<CameraModel>>>();

            return json!;
        }

        public async Task<ApiResponse<IEnumerable<CameraLogModel>>> GetLogs(int cameraId)
        {
            var result = await _http.GetAsync($"api/camera/logs/{cameraId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<CameraLogModel>>>();

            return json!;
        }
    }
}