
using GodsEye.Shared.Response;
using GodsEye.Shared.Response.NotificationGroups;
using GodsEye.WEB.Model.Forms;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class NotificationGroupWebService
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/notification-group";

        public NotificationGroupWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ProcedureResponse?> CreateAsync(CreateNotificationGroupForm notificationGroup)
        {
            var result = await _http.PostAsJsonAsync(_baseEndpoint, notificationGroup);

            var json = await result.Content.ReadFromJsonAsync<ProcedureResponse?>();

            return json!;
        }

        public async Task<NotificationGroupsResponse> GetById(int notificationGroupId)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{notificationGroupId}");

            var json = await result.Content.ReadFromJsonAsync<NotificationGroupsResponse>();

            return json!;
        }

        public async Task<IEnumerable<NotificationGroupsResponse>> GetAllAsync()
        {
            var result = await _http.GetAsync(_baseEndpoint);

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<NotificationGroupsResponse>>();

            return json!;
        }

        public async Task<int> UpdateAsync(UpdateNotificationGroupForm notificationGroup)
        {
            var result = await _http.PutAsJsonAsync(_baseEndpoint, notificationGroup);

            var json = await result.Content.ReadFromJsonAsync<int>();

            return json!;
        }
    }
}
