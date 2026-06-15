namespace RivenBackend.DTOs
{
    public class RiskFactorsDto
    {
        public int RiskFactorId { get; set; }
        public int CaseId { get; set; }
        public bool PreviousStroke { get; set; }
        public bool Hypertension { get; set; }
        public bool Diabetes { get; set; }
        public bool HeartDisease { get; set; }
        public bool HighCholesterol { get; set; }
        public bool Smoking { get; set; }
        public bool Obesity { get; set; }
        public bool SleepApnea { get; set; }
        public bool PhysicalInactive { get; set; }
    }

    public class CreateRiskFactorsDto
    {
        public int CaseId { get; set; }
        public bool PreviousStroke { get; set; }
        public bool Hypertension { get; set; }
        public bool Diabetes { get; set; }
        public bool HeartDisease { get; set; }
        public bool HighCholesterol { get; set; }
        public bool Smoking { get; set; }
        public bool Obesity { get; set; }
        public bool SleepApnea { get; set; }
        public bool PhysicalInactive { get; set; }
    }
}