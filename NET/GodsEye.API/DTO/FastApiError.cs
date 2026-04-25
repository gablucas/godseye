using System.Text.Json.Serialization;

namespace GodsEye.API.DTO
{
    public class FastApiError
    {
        [JsonPropertyName("detail")]
        public string? Detail { get; set; }
    }
}
