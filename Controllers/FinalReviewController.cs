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
    public class FinalReviewController : ControllerBase
    {
        private readonly IFinalReviewService _finalReviewService;
        private readonly IAiReportService _aiReportService;

        public FinalReviewController(IFinalReviewService finalReviewService, IAiReportService aiReportService)
        {
            _finalReviewService = finalReviewService;
            _aiReportService = aiReportService;
        }

        // GET: api/finalreview/cases/{caseId}
        [HttpGet("cases/{caseId}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<FinalReviewResponseDto>> GetFinalReview(int caseId)
        {
            try
            {
                var result = await _finalReviewService.GetFinalReviewAsync(caseId);
                if (result == null) return NotFound(new ApiResponse { Success = false, Message = "Case not found." });
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // POST: api/finalreview/cases/{caseId}
        [HttpPost("cases/{caseId}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<FinalReviewResponseDto>> SubmitFinalReview(
            int caseId,
            [FromForm] FinalReviewRequestDto dto,
            [FromForm] List<IFormFile>? images)
        {
            try
            {
                var result = await _finalReviewService.SubmitFinalReviewAsync(caseId, dto, images);
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

        // POST: api/finalreview/cases/{caseId}/generate-report
        [HttpPost("cases/{caseId}/generate-report")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<ActionResult<FullMedicalReportDto>> GenerateReport(int caseId)
        {
            try
            {
                var aiReport = await _aiReportService.GenerateReportAsync(caseId);
                if (aiReport == null) return NotFound(new ApiResponse { Success = false, Message = "Case not found." });

                var fullReport = await _aiReportService.GetFullReportAsync(caseId);
                return Ok(fullReport);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
