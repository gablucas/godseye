using GodsEye.Shared.Response.Compliance;
using GodsEye.WEB.Model.Forms;
using GodsEye.WEB.Services.Interfaces;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class ComplianceWebService : IWebService<CompliancePolicyResponse>
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/compliance";

        public ComplianceWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<int> CreateAsync(ComplianceSectorTransitionRule rule)
        {
            var result = await _http.PostAsJsonAsync($"{_baseEndpoint}/rule/sector-transitions", rule);

            var json = await result.Content.ReadFromJsonAsync<int>();

            return json!;
        }

        public async Task<IEnumerable<CompliancePolicyResponse>> GetAllAsync()
        {
            var result = await _http.GetAsync(_baseEndpoint);

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<CompliancePolicyResponse>>();

            return json!;
        }

        public async Task<CompliancePolicyResponse> GetById(int id)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{id}");

            var json = await result.Content.ReadFromJsonAsync<CompliancePolicyResponse>();

            return json!;
        }

        

        public async Task<IEnumerable<ComplianceViolationResponse>> GetAllViolationsAsync()
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/violation");

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<ComplianceViolationResponse>>();

            return json!;
        }

        public async Task<SectorTransitionResponse> GetSectorTransitionById(int id)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/sector-transition/{id}");

            var json = await result.Content.ReadFromJsonAsync<SectorTransitionResponse>();

            return json!;
        }
    }

    public class ComplianceViolationWebService : IWebService<ComplianceViolationResponse>
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/compliance/violation";

        public ComplianceViolationWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<ComplianceViolationResponse>> GetAllAsync()
        {
            var result = await _http.GetAsync($"{_baseEndpoint}");

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<ComplianceViolationResponse>>();

            return json!;
        }

        public async Task<ComplianceViolationResponse> GetById(int id)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{id}");

            var json = await result.Content.ReadFromJsonAsync<ComplianceViolationResponse>();

            return json!;
        }
    }
}
