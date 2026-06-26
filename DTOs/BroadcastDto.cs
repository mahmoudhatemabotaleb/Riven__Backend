public class BroadcastDto
{
    public int HospitalId { get; set; }
    public string EmergencyType { get; set; } = string.Empty;
    public string SeverityLevel { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<int>? TargetUserIds { get; set; } // null = send to all
}