using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RivenBackend.DTOs;
using RivenBackend.Models;
using RivenBackend.Services;

namespace RivenBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TransportController : ControllerBase
    {
        private readonly ITransportService _transportService;

        public TransportController(ITransportService transportService)
        {
            _transportService = transportService;
        }

        [HttpGet("cases/{caseId}/status")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<TransportStatusDto>> GetTransportStatus(int caseId)
        {
            try
            {
                var result = await _transportService.GetTransportStatusAsync(caseId);
                if (result == null) return NotFound(new ApiResponse { Success = false, Message = "Case not found." });
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpGet("cases/{caseId}/navigation")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<NavigationDto>> GetNavigation(int caseId)
        {
            try
            {
                var result = await _transportService.GetNavigationAsync(caseId);
                if (result == null) return NotFound(new ApiResponse { Success = false, Message = "Case not found." });
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPatch("cases/{caseId}/arrived")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<ArrivedAtHospitalResponseDto>> MarkArrived(int caseId, [FromBody] ArrivedAtHospitalDto dto)
        {
            try
            {
                var result = await _transportService.MarkArrivedAsync(caseId, dto);
                if (result == null) return NotFound(new ApiResponse { Success = false, Message = "Case not found." });
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse { Success = false, Message = ex.Message });
            }
        }
    }
}
