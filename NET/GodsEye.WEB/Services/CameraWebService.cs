using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.WEB.Model.Forms;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class CameraWebService
    {
        private readonly HttpClient _http;

        public CameraWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResponse<ProcedureResult?>> CreateAsync(CreateCameraForm camera)
        {
            var result = await _http.PostAsJsonAsync("api/camera", camera);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<ProcedureResult?>>();

            return json!;
        }

        public async Task<ApiResponse<ProcedureResult?>> UpdateAsync(UpdateCameraForm camera)
        {
            var result = await _http.PutAsJsonAsync("api/camera", camera);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<ProcedureResult?>>();

            return json!;
        }

        public async Task<ApiResponse<IEnumerable<CameraModel>>> GetAllAsync()
        {
            var result = await _http.GetAsync("api/camera");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<CameraModel>>>();

            return json!;
        }

        public async Task<ApiResponse<CameraModel>> GetById(int cameraId)
        {
            var result = await _http.GetAsync($"api/camera/{cameraId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<CameraModel>>();

            return json!;
        }

        public async Task<ApiResponse<IEnumerable<CameraLogModel>>> GetLogs(int cameraId)
        {
            var result = await _http.GetAsync($"api/camera/logs/{cameraId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<CameraLogModel>>>();

            return json!;
        }

        public async Task<IEnumerable<CameraByFeatureModel>> GetByFeatureId(int featureId)
        {
            var response = await _http.GetAsync($"api/camera/feature/{featureId}");
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content
                .ReadFromJsonAsync<ApiResponse<IEnumerable<CameraByFeatureModel>>>();

            return apiResponse?.Data ?? Enumerable.Empty<CameraByFeatureModel>();
        }

        public async Task<IEnumerable<CameraFeatureModel>> GetFeatures(int cameraId)
        {
            var response = await _http.GetAsync($"api/camera/active-features/{cameraId}");
            response.EnsureSuccessStatusCode();

            var apiResponse = await response.Content
                .ReadFromJsonAsync<ApiResponse<IEnumerable<CameraFeatureModel>>>();

            return apiResponse?.Data ?? Enumerable.Empty<CameraFeatureModel>();
        }
    }
}