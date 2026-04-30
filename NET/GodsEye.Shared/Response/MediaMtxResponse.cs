using System.Text.Json.Serialization;

namespace GodsEye.API.DTOs.Response
{
    public class MediaMtxResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("error")]

        public string? Error { get; set; }
    }
}
