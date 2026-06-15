namespace RivenBackend.DTOs
{
    public class TransportStatusDto
    {
        public int CaseId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int EstimatedArrivalMinutes { get; set; }
        public double DistanceMiles { get; set; }
        public HospitalTransportDto Hospital { get; set; } = null!;
        public AmbulanceLocationDto Ambulance { get; set; } = null!;
        public PatientSummaryDto PatientSummary { get; set; } = null!;
        public HospitalPreparationDto Preparation { get; set; } = null!;
    }

    public class NavigationDto
    {
        public int CaseId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int EstimatedArrivalMinutes { get; set; }
        public string HospitalName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public double HospitalLatitude { get; set; }
        public double HospitalLongitude { get; set; }
        public double? AmbulanceLatitude { get; set; }
        public double? AmbulanceLongitude { get; set; }
        public bool CanMarkArrived { get; set; }
    }

    public class HospitalTransportDto
    {
        public int HospitalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StrokeCenterType { get; set; } = string.Empty;
        public int WaitTimeMinutes { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
    }

    public class AmbulanceLocationDto
    {
        public int AmbulanceId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public double? CurrentLatitude { get; set; }
        public double? CurrentLongitude { get; set; }
        public int? EtaMinutes { get; set; }
        public double? DistanceMiles { get; set; }
    }

    public class PatientSummaryDto
    {
        public string? AiPrediction { get; set; }
        public int SymptomOnsetMinutesAgo { get; set; }
        public string? BloodPressure { get; set; }
        public string? PatientName { get; set; }
    }

    public class HospitalPreparationDto
    {
        public bool StrokeTeamNotified { get; set; }
        public bool EmergencyBayCleared { get; set; }
        public bool NeurologistOnStandby { get; set; }
    }

    public class ArrivedAtHospitalDto
    {
        public string? PatientConditionOnArrival { get; set; }
        public string? Notes { get; set; }
    }

    public class ArrivedAtHospitalResponseDto
    {
        public int CaseId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime ArrivedTime { get; set; }
        public string? HospitalName { get; set; }
    }
}
