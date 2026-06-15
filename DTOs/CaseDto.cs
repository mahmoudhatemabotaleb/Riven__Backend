namespace RivenBackend.DTOs
{
    public class CaseDto
    {
        public int CaseId { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int UserId { get; set; }
        public int AmbulanceId { get; set; }
        public int HospitalId { get; set; }
        public string? HospitalName { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime OnsetTime { get; set; }
        public DateTime CaseDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public double? LocationLatitude { get; set; }
        public double? LocationLongitude { get; set; }
        public DateTime? ArrivedTime { get; set; }
        public DateTime? HandoverTime { get; set; }
        public string? ReceivingPhysician { get; set; }
        public string? PatientConditionOnArrival { get; set; }
        public string? HandoverNotes { get; set; }
    }

    public class CreateCaseDto
    {
        public int PatientId { get; set; }
        public int UserId { get; set; }
        public int AmbulanceId { get; set; }
        public int HospitalId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime OnsetTime { get; set; }
        public DateTime CaseDate { get; set; }
        public string Location { get; set; } = string.Empty;
    }

    public class UpdateCaseStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }

    public class HandoverDto
    {
        public string ReceivingPhysician { get; set; } = string.Empty;
        public string PatientConditionOnArrival { get; set; } = string.Empty;
        public string? HandoverNotes { get; set; }
    }

    public class HandoverSummaryDto
    {
        public int CaseId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? ArrivedTime { get; set; }
        public DateTime? HandoverTime { get; set; }
        public string? ReceivingPhysician { get; set; }
        public string? PatientConditionOnArrival { get; set; }
        public string? HandoverNotes { get; set; }
        public string? HospitalName { get; set; }
        public string? PatientName { get; set; }
    }

    public class DashboardDto
    {
        public int HospitalId { get; set; }
        public int TotalCases { get; set; }
        public int ActiveCases { get; set; }
        public int EnRouteCases { get; set; }
        public int ArrivedCases { get; set; }
        public int CompletedCases { get; set; }
        public int TodayCases { get; set; }
        public double AverageOnsetToArrivalMinutes { get; set; }
        public int UnreadNotifications { get; set; }
    }

    public class CaseDetailDto
    {
        public CaseDto Case { get; set; } = null!;
        public PatientDto? Patient { get; set; }
        public VitalSignsDto? VitalSigns { get; set; }
        public SymptomsDto? Symptoms { get; set; }
        public RiskFactorsDto? RiskFactors { get; set; }
        public NihssAssessmentDto? NihssAssessment { get; set; }
        public AiReportDto? AiReport { get; set; }
        public List<MedicationDto> Medications { get; set; } = [];
        public List<AttachmentDto> Attachments { get; set; } = [];
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    }
}
