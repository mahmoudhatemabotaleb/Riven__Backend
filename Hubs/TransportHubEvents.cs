namespace RivenBackend.Hubs
{
    public static class TransportHubEvents
    {
        public const string LocationUpdated = "LocationUpdated";
        public const string CaseStatusUpdated = "CaseStatusUpdated";
        public const string HospitalNotification = "HospitalNotification";
    }

    public class AmbulanceLocationUpdateMessage
    {
        public int AmbulanceId { get; set; }
        public int HospitalId { get; set; }
        public int? CaseId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? EtaMinutes { get; set; }
        public double? DistanceMiles { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CaseStatusUpdateMessage
    {
        public int CaseId { get; set; }
        public int HospitalId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? ArrivedTime { get; set; }
    }
}
