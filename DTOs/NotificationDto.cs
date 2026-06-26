namespace RivenBackend.DTOs
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public int HospitalId { get; set; }
        public int? CaseId { get; set; }
        public int? UserId { get; set; }
        public DateTime SentTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string? Type { get; set; }
        public bool IsRead { get; set; }
        public string? PatientName { get; set; }
        public string? CaseLocation { get; set; }
        public string? CaseSeverity { get; set; }
        public string? AiDiagnosis { get; set; }
    }

    public class CreateNotificationDto
    {
        public int HospitalId { get; set; }
        public int? CaseId { get; set; }
        public int? UserId { get; set; }
        public DateTime SentTime { get; set; } = DateTime.UtcNow;  // ← fixed
        public string Status { get; set; } = "Pending";
        public string? Message { get; set; }
        public string? Type { get; set; }
    }

    public class UpdateNotificationStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public bool IsRead { get; set; }
    }

    public class BroadcastDto
    {
        public int HospitalId { get; set; }
        public string EmergencyType { get; set; } = string.Empty;
        public string SeverityLevel { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public List<int>? TargetUserIds { get; set; }
    }
}