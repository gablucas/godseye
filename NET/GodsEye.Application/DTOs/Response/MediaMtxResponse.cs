using System.Text.Json.Serialization;

namespace GodsEye.Application.DTOs.Response
{
    public class MediaMtxResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("error")]

        public string? Error { get; set; }
    }
}
