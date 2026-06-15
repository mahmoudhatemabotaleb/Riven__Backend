using RivenBackend.DTOs;
using RivenBackend.Models;

namespace RivenBackend.Mappings
{
    public static class HospitalMapper
    {
        public static HospitalTransportDto ToTransportDto(Hospital hospital) => new()
        {
            HospitalId = hospital.HospitalId,
            Name = hospital.Name,
            Status = hospital.Status,
            StrokeCenterType = hospital.StrokeCenterType,
            WaitTimeMinutes = hospital.WaitTimeMinutes,
            Latitude = hospital.Latitude,
            Longitude = hospital.Longitude,
            ContactNumber = hospital.ContactNumber
        };

        public static HospitalPreparationDto ToPreparationDto(Hospital hospital) => new()
        {
            StrokeTeamNotified = hospital.StrokeTeamNotified,
            EmergencyBayCleared = hospital.EmergencyBayCleared,
            NeurologistOnStandby = hospital.NeurologistOnStandby
        };
    }
}
