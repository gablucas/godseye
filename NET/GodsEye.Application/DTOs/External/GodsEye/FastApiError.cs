using System.Text.Json.Serialization;

namespace GodsEye.Application.DTOs.External.GodsEye
{
    public class FastApiError
    {
        [JsonPropertyName("detail")]
        public string? Detail { get; set; }
    }
}
