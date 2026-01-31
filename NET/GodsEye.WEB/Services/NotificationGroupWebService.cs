using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Domain.DTOs.Result;
using GodsEye.WEB.Model.Forms;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class NotificationGroupWebService
    {
        private readonly HttpClient _http;

        public NotificationGroupWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ApiResponse<ProcedureResult?>> CreateAsync(CreateNotificationGroupForm notificationGroup)
        {
            var result = await _http.PostAsJsonAsync("api/notificationgroup", notificationGroup);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<ProcedureResult?>>();

            return json!;
        }

        public async Task<ApiResponse<NotificationGroupModel>> GetById(int notificationGroupId)
        {
            var result = await _http.GetAsync($"api/notificationgroup/{notificationGroupId}");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<NotificationGroupModel>>();

            return json!;
        }

        public async Task<ApiResponse<IEnumerable<NotificationGroupModel>>> GetAllAsync()
        {
            var result = await _http.GetAsync("api/notificationgroup");

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<NotificationGroupModel>>>();

            return json!;
        }

        public async Task<ApiResponse<int>> UpdateAsync(UpdateNotificationGroupForm notificationGroup)
        {
            var result = await _http.PutAsJsonAsync("api/notificationgroup", notificationGroup);

            var json = await result.Content.ReadFromJsonAsync<ApiResponse<int>>();

            return json!;
        }
    }
}
