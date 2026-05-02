using GodsEye.Shared.Response.Camera;
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

        public async Task<int> CreateAsync(CreateCameraForm camera)
        {
            var result = await _http.PostAsJsonAsync(_baseEndpoint, camera);

            var json = await result.Content.ReadFromJsonAsync<int>();

            return json!;
        }

        public async Task<int> UpdateAsync(UpdateCameraForm camera)
        {
            var result = await _http.PutAsJsonAsync(_baseEndpoint, camera);

            var json = await result.Content.ReadFromJsonAsync<int>();

            return json!;
        }

        public async Task<IEnumerable<CameraResponse>> GetAllAsync()
        {
            var result = await _http.GetAsync(_baseEndpoint);

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<CameraResponse>>();

            return json!;
        }

        public async Task<CameraResponse> GetById(int cameraId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{cameraId}");

            var json = await result.Content.ReadFromJsonAsync<CameraResponse>();

            return json!;
        }

        public async Task<IEnumerable<CameraLogResponse>> GetLogs(int cameraId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/logs/{cameraId}");

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<CameraLogResponse>>();

            return json!;
        }

        public async Task<IEnumerable<CameraFeatureResponse>> GetByFeatureId(int featureId)
        {
            var response = await _http.GetAsync($"{_baseEndpoint}/feature/{featureId}");

            var apiResponse = await response.Content
                .ReadFromJsonAsync<IEnumerable<CameraFeatureResponse>>();

            return apiResponse ?? Enumerable.Empty<CameraFeatureResponse>();
        }

        public async Task<IEnumerable<CameraFeatureResponse>> GetFeatures(int cameraId)
        {
            var response = await _http.GetAsync($"{_baseEndpoint}/active-features/{cameraId}");

            var apiResponse = await response.Content
                .ReadFromJsonAsync<IEnumerable<CameraFeatureResponse>>();

            return apiResponse ?? Enumerable.Empty<CameraFeatureResponse>();
        }

        public async Task<List<CameraRoiResponse>> GetRoiByCameraId(int cameraId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/roi/{cameraId}");

            var json = await result.Content.ReadFromJsonAsync<List<CameraRoiResponse>>();

            return json!;
        }

        public async Task<int> CreateRoiAsync(CameraRoiForm cameraRoi)
        {
            var result = await _http.PostAsJsonAsync($"{_baseEndpoint}/roi", cameraRoi);

            var json = await result.Content.ReadFromJsonAsync<int>();

            return json!;
        }

        public async Task<int> UpdateRoiAsync(CameraRoiForm cameraRoi)
        {
            var result = await _http.PutAsJsonAsync($"{_baseEndpoint}/roi", cameraRoi);

            var json = await result.Content.ReadFromJsonAsync<int>();

            return json!;
        }

       public async Task<int> DeleteRoiAsync(int roiId)
        {
            var result = await _http.DeleteAsync($"{_baseEndpoint}/roi/{roiId}");

            var json = await result.Content.ReadFromJsonAsync<int>();

            return json!;
        }

        public async Task<CameraIncidentRecordingForm> GetConfigIncidentRecording(int cameraId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/config/incident-recording/{cameraId}");

            var json = await result.Content.ReadFromJsonAsync<CameraIncidentRecordingForm>();

            return json!;
        }

        public async Task<int> UpdateConfigIncidentRecording(CameraIncidentRecordingForm incidentRecordingCamera)
        {
            var result = await _http.PutAsJsonAsync($"{_baseEndpoint}/config/incident-recording", incidentRecordingCamera);

            var json = await result.Content.ReadFromJsonAsync<int>();

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

            var apiResponse = await response.Content.ReadFromJsonAsync<string>();

            return apiResponse is not null;
        }
    }
}