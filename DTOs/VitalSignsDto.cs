namespace RivenBackend.DTOs
{
    public class VitalSignsDto
    {
        public int VitalId { get; set; }
        public int CaseId { get; set; }
        public double SpO2 { get; set; }
        public int SystolicBP { get; set; }
        public int DiastolicBP { get; set; }
        public double HeartRate { get; set; }
        public double Temperature { get; set; }
        public string TemperatureUnit { get; set; } = "F";
        public double RespiratoryRate { get; set; }
        public double GlucoseLevel { get; set; }
    }

    public class CreateVitalSignsDto
    {
        public int CaseId { get; set; }
        public double SpO2 { get; set; }
        public int SystolicBP { get; set; }
        public int DiastolicBP { get; set; }
        public double HeartRate { get; set; }
        public double Temperature { get; set; }
        public string TemperatureUnit { get; set; } = "F";
        public double RespiratoryRate { get; set; }
        public double GlucoseLevel { get; set; }
    }
}