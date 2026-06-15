using System.ComponentModel.DataAnnotations;

namespace RivenBackend.DTOs
{
    public class FinalReviewRequestDto
    {
        [Required]
        [Range(1, 1440)]
        public int SymptomOnsetMinutes { get; set; }

        public string? Location { get; set; }

        public bool UseCurrentLocation { get; set; }

        public double? LocationLatitude { get; set; }

        public double? LocationLongitude { get; set; }

        public string? AdditionalNotes { get; set; }
    }

    public class FinalReviewResponseDto
    {
        public int CaseId { get; set; }
        public DateTime OnsetTime { get; set; }
        public int SymptomOnsetMinutesAgo { get; set; }
        public string Location { get; set; } = string.Empty;
        public double? LocationLatitude { get; set; }
        public double? LocationLongitude { get; set; }
        public string? AdditionalNotes { get; set; }
        public List<AttachmentDto> UploadedImages { get; set; } = [];
    }
}
