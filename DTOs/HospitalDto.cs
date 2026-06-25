namespace RivenBackend.DTOs
{
    // ── Hospital DTOs ──────────────────────────────────────────
    public class HospitalDto
    {
        public int HospitalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? CityStateZip { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
        public string StrokeCenterType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int AvailableStrokeBeds { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int WaitTimeMinutes { get; set; }
        public bool StrokeTeamNotified { get; set; }
        public bool EmergencyBayCleared { get; set; }
        public bool NeurologistOnStandby { get; set; }
        public string? ProfilePicture { get; set; }
    }

    public class CreateHospitalDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? CityStateZip { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
        public string StrokeCenterType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int AvailableStrokeBeds { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int WaitTimeMinutes { get; set; }
        public bool StrokeTeamNotified { get; set; }
        public bool EmergencyBayCleared { get; set; }
        public bool NeurologistOnStandby { get; set; }
        public string? ProfilePicture { get; set; }
    }

    public class UpdateHospitalPreparationDto
    {
        public bool StrokeTeamNotified { get; set; }
        public bool EmergencyBayCleared { get; set; }
        public bool NeurologistOnStandby { get; set; }
    }

    public class UpdateHospitalSettingsDto
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? CityStateZip { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
        public int AvailableStrokeBeds { get; set; }
        public string? ProfilePicture { get; set; }
    }

    // ── Account Settings DTOs ──────────────────────────────────
    public class AccountSettingsDto
    {
        public string Email { get; set; } = string.Empty;
    }

    public class UpdateAccountSettingsDto
    {
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
    }
}