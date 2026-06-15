using RivenBackend.Constants;
using RivenBackend.DTOs;
using RivenBackend.Models;

namespace RivenBackend.Mappings
{
    public static class TransportMapper
    {
        public static TransportStatusDto ToTransportStatusDto(Case case_)
        {
            var hospital = case_.Hospital;
            var ambulance = case_.Ambulance;
            var vitals = case_.VitalSigns;
            var aiReport = case_.AiReport;

            return new TransportStatusDto
            {
                CaseId = case_.CaseId,
                Status = case_.Status,
                EstimatedArrivalMinutes = ambulance.EtaMinutes ?? 0,
                DistanceMiles = ambulance.DistanceMiles ?? 0,
                Hospital = HospitalMapper.ToTransportDto(hospital),
                Ambulance = ToAmbulanceLocationDto(ambulance),
                PatientSummary = ToPatientSummary(case_, vitals, aiReport),
                Preparation = HospitalMapper.ToPreparationDto(hospital)
            };
        }

        public static NavigationDto ToNavigationDto(Case case_)
        {
            var hospital = case_.Hospital;
            var ambulance = case_.Ambulance;
            var canMarkArrived = case_.Status is CaseStatuses.EnRoute or CaseStatuses.Active;

            return new NavigationDto
            {
                CaseId = case_.CaseId,
                Status = case_.Status,
                EstimatedArrivalMinutes = ambulance.EtaMinutes ?? 0,
                HospitalName = hospital.Name,
                ContactNumber = hospital.ContactNumber,
                HospitalLatitude = hospital.Latitude,
                HospitalLongitude = hospital.Longitude,
                AmbulanceLatitude = ambulance.CurrentLatitude,
                AmbulanceLongitude = ambulance.CurrentLongitude,
                CanMarkArrived = canMarkArrived
            };
        }

        public static AmbulanceLocationDto ToAmbulanceLocationDto(Ambulance ambulance) => new()
        {
            AmbulanceId = ambulance.AmbulanceId,
            VehicleNumber = ambulance.VehicleNumber,
            CurrentLatitude = ambulance.CurrentLatitude,
            CurrentLongitude = ambulance.CurrentLongitude,
            EtaMinutes = ambulance.EtaMinutes,
            DistanceMiles = ambulance.DistanceMiles
        };

        private static PatientSummaryDto ToPatientSummary(Case case_, VitalSigns? vitals, AiReport? aiReport)
        {
            var onsetMinutesAgo = (int)Math.Max(0, (DateTime.UtcNow - case_.OnsetTime.ToUniversalTime()).TotalMinutes);
            var bloodPressure = vitals != null
                ? $"{vitals.SystolicBP}/{vitals.DiastolicBP} mmHg"
                : null;

            return new PatientSummaryDto
            {
                AiPrediction = aiReport?.StrokeType,
                SymptomOnsetMinutesAgo = onsetMinutesAgo,
                BloodPressure = bloodPressure,
                PatientName = case_.Patient?.Name
            };
        }
    }
}
