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
                PatientAge = c.Patient.Age,
                PatientGender = c.Patient.Gender,
                UserId = c.UserId,
                AmbulanceId = c.AmbulanceId,
                HospitalId = c.HospitalId,
                HospitalName = c.Hospital.Name,
                Status = c.Status,
                Severity = c.Severity,
                AiPrediction = c.AiReport != null ? c.AiReport.StrokeType : null,
                ParamedicName = c.User.FirstName + " " + c.User.LastName,
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
            PatientAge = c.Patient?.Age,
            PatientGender = c.Patient?.Gender,
            UserId = c.UserId,
            AmbulanceId = c.AmbulanceId,
            HospitalId = c.HospitalId,
            HospitalName = c.Hospital?.Name,
            Status = c.Status,
            Severity = c.Severity,
            AiPrediction = c.AiReport?.StrokeType,
            ParamedicName = c.User != null ? c.User.FullName : null,
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