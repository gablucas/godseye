using GodsEye.Application.DTOs.Model;
using GodsEye.Application.DTOs.Response;
using GodsEye.Application.UseCases.Compliance.Commands;
using GodsEye.WEB.Services.Interfaces;
using System.Net.Http.Json;

namespace GodsEye.WEB.Services
{
    public class ComplianceWebService : IWebService<CompliancePolicyDTO>
    {
        private readonly HttpClient _http;
        private readonly string _baseEndpoint = "api/compliance";

        public ComplianceWebService(HttpClient http)
        {
            _http = http;
        }

        public async Task<int> CreateAsync(CreateSectorTransitionRuleRequest rule)
        {
            var result = await _http.PostAsJsonAsync($"{_baseEndpoint}/rule/sector-transitions", rule);

            var json = await result.Content.ReadFromJsonAsync<int>();

            return json!;
        }

        public async Task<IEnumerable<CompliancePolicyDTO>> GetAllAsync()
        {
            var result = await _http.GetAsync(_baseEndpoint);

            var json = await result.Content.ReadFromJsonAsync<IEnumerable<CompliancePolicyDTO>>();

            return json!;
        }

        public async Task<CompliancePolicyDTO> GetById(int id)
        {
            var result = await _http.GetAsync($"{_baseEndpoint}/{id}");

            var json = await result.Content.ReadFromJsonAsync<CompliancePolicyDTO>();

            return json!;
        }
    }
}
