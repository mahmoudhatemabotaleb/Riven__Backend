using RivenBackend.DTOs;
using RivenBackend.Models;
using System.Linq.Expressions;

namespace RivenBackend.Mappings
{
    public static class CaseMapper
    {
        public static Expression<Func<Case, CaseDto>> ToDtoExpression =>
            c => new CaseDto
            {
                CaseId = c.CaseId,
                PatientId = c.PatientId,
                PatientName = c.Patient.Name,
                UserId = c.UserId,
                AmbulanceId = c.AmbulanceId,
                HospitalId = c.HospitalId,
                HospitalName = c.Hospital.Name,
                Status = c.Status,
                Severity = c.Severity,
                OnsetTime = c.OnsetTime,
                CaseDate = c.CaseDate,
                Location = c.Location,
                LocationLatitude = c.LocationLatitude,
                LocationLongitude = c.LocationLongitude,
                ArrivedTime = c.ArrivedTime,
                HandoverTime = c.HandoverTime,
                ReceivingPhysician = c.ReceivingPhysician,
                PatientConditionOnArrival = c.PatientConditionOnArrival,
                HandoverNotes = c.HandoverNotes
            };

        public static CaseDto ToDto(Case c) => new()
        {
            CaseId = c.CaseId,
            PatientId = c.PatientId,
            PatientName = c.Patient?.Name,
            UserId = c.UserId,
            AmbulanceId = c.AmbulanceId,
            HospitalId = c.HospitalId,
            HospitalName = c.Hospital?.Name,
            Status = c.Status,
            Severity = c.Severity,
            OnsetTime = c.OnsetTime,
            CaseDate = c.CaseDate,
            Location = c.Location,
            LocationLatitude = c.LocationLatitude,
            LocationLongitude = c.LocationLongitude,
            ArrivedTime = c.ArrivedTime,
            HandoverTime = c.HandoverTime,
            ReceivingPhysician = c.ReceivingPhysician,
            PatientConditionOnArrival = c.PatientConditionOnArrival,
            HandoverNotes = c.HandoverNotes
        };
    }
}
