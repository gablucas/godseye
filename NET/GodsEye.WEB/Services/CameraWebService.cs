using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.UseCases.Camera.Commands.CreateCameraRoi;
using GodsEye.Application.UseCases.Camera.Commands.DeleteCameraRoi;
using GodsEye.Application.UseCases.Camera.Commands.UpdateCameraRoi;
using GodsEye.Application.UseCases.Camera.Queries.GetCamerasRoiByCameraId;
using GodsEye.Domain.DTOs.Result;
using GodsEye.WEB.Model.Forms;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class CameraWebService
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/camera";

        public CameraWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResponse<ProcedureResult?>> CreateAsync(CreateCameraForm camera)
        {
            var result = await _http.PostAsJsonAsync(_baseEndpoint, camera);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<ProcedureResult?>>();

            return json!;
        }

        public async Task<ApiResponse<ProcedureResult?>> UpdateAsync(UpdateCameraForm camera)
        {
            var result = await _http.PutAsJsonAsync(_baseEndpoint, camera);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<ProcedureResult?>>();

            return json!;
        }

        public async Task<ApiResponse<IEnumerable<CameraModel>>> GetAllAsync()
        {
            var result = await _http.GetAsync(_baseEndpoint);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<CameraModel>>>();

            return json!;
        }

        public async Task<ApiResponse<CameraModel>> GetById(int cameraId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{cameraId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<CameraModel>>();

            return json!;
        }

        public async Task<ApiResponse<IEnumerable<CameraLogModel>>> GetLogs(int cameraId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/logs/{cameraId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<CameraLogModel>>>();

            return json!;
        }

        public async Task<IEnumerable<CameraByFeatureModel>> GetByFeatureId(int featureId)
        {
            var response = await _http.GetAsync($"{_baseEndpoint}/feature/{featureId}");

            var apiResponse = await response.Content
                .ReadFromJsonAsync<ApiResponse<IEnumerable<CameraByFeatureModel>>>();

            return apiResponse?.Data ?? Enumerable.Empty<CameraByFeatureModel>();
        }

        public async Task<IEnumerable<CameraFeatureModel>> GetFeatures(int cameraId)
        {
            var response = await _http.GetAsync($"{_baseEndpoint}/active-features/{cameraId}");

            var apiResponse = await response.Content
                .ReadFromJsonAsync<ApiResponse<IEnumerable<CameraFeatureModel>>>();

            return apiResponse?.Data ?? Enumerable.Empty<CameraFeatureModel>();
        }

        public async Task<ApiResponse<List<CameraRoiModel>>> GetRoiByCameraId(int cameraId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/roi/{cameraId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<List<CameraRoiModel>>>();

            return json!;
        }

        public async Task<ApiResponse<int>> CreateRoiAsync(CreateCameraRoiRequest cameraRoi)
        {
            var result = await _http.PostAsJsonAsync($"{_baseEndpoint}/roi", cameraRoi);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<int>>();

            return json!;
        }

        public async Task<ApiResponse<int>> UpdateRoiAsync(UpdateCameraRoiRequest cameraRoi)
        {
            var result = await _http.PutAsJsonAsync($"{_baseEndpoint}/roi", cameraRoi);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<int>>();

            return json!;
        }

        public async Task<ApiResponse<int>> DeelteRoiAsync(int roiId)
        {
            var result = await _http.DeleteAsync($"{_baseEndpoint}/roi/{roiId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<int>>();

            return json!;
        }

        public async Task<bool> TesteCameraConnection(string rtspUrl)
        {
            var payload = new { rtspUrl };

            HttpResponseMessage response;

            try
            {
                response = await _http.PostAsJsonAsync(
                    $"{_baseEndpoint}/test-connection",
                    payload
                );
            }
            catch (HttpRequestException)
            {
                // API offline / CORS / DNS
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                // aqui cai 400, 401, 405, 500 etc
                return false;
            }

            var apiResponse =
                await response.Content.ReadFromJsonAsync<ApiResponse<string>>();

            return apiResponse?.Success == true;
        }
    }
}