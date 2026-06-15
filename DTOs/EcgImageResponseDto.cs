using System.Text.Json.Serialization;

namespace RivenBackend.DTOs
{
    public class EcgImageResponseDto
    {
        [JsonPropertyName("class")]
        public string ClassName { get; set; } = string.Empty;

        [JsonPropertyName("confidence")]
        public string Confidence { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}
