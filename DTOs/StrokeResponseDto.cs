using System.Text.Json.Serialization;

namespace RivenBackend.DTOs
{
    public class StrokeResponseDto
    {
        [JsonPropertyName("total_images_processed")]
        public int TotalImagesProcessed { get; set; }

        [JsonPropertyName("patient_final_diagnosis")]
        public string PatientFinalDiagnosis { get; set; } = string.Empty;

        [JsonPropertyName("confidence")]
        public string Confidence { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }
}
