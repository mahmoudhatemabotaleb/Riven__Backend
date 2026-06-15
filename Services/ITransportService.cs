using RivenBackend.DTOs;

namespace RivenBackend.Services
{
    public interface ITransportService
    {
        Task<TransportStatusDto?> GetTransportStatusAsync(int caseId);
        Task<NavigationDto?> GetNavigationAsync(int caseId);
        Task<ArrivedAtHospitalResponseDto?> MarkArrivedAsync(int caseId, ArrivedAtHospitalDto dto);
    }
}
