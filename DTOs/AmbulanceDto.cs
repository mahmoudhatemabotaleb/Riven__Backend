namespace RivenBackend.DTOs
{
    public class AmbulanceDto
    {
        public int AmbulanceId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string AmbulanceType { get; set; } = string.Empty;
        public string OperationalStatus { get; set; } = string.Empty;
        public int HospitalId { get; set; }
        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }
        public int? EtaMinutes { get; set; }
        public double? DistanceMiles { get; set; }
    }

    public class CreateAmbulanceDto
    {
        public string VehicleNumber { get; set; } = string.Empty;
        public string AmbulanceType { get; set; } = string.Empty;
        public string OperationalStatus { get; set; } = string.Empty;
        public int HospitalId { get; set; }
        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }
        public int? EtaMinutes { get; set; }
        public double? DistanceMiles { get; set; }
    }

    public class UpdateAmbulanceLocationDto
    {
        public double CurrentLatitude { get; set; }
        public double CurrentLongitude { get; set; }
        public int EtaMinutes { get; set; }
        public double DistanceMiles { get; set; }
    }
}