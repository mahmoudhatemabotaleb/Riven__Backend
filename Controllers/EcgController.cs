using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RivenBackend.Data;
using RivenBackend.DTOs;
using RivenBackend.Models;
using RivenBackend.Security;
using System.Text.Json;

namespace RivenBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EcgController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        private readonly ICaseAccessService _caseAccess;

        public EcgController(
            IHttpClientFactory httpClientFactory,
            AppDbContext context,
            ICaseAccessService caseAccess)
        {
            _httpClient = httpClientFactory.CreateClient();
            _context = context;
            _caseAccess = caseAccess;
        }

        [HttpPost("analyze")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<IActionResult> AnalyzeEcg([FromForm] int caseId, List<IFormFile> files)
        {
            var denied = await AuthorizeCaseAsync(caseId);
            if (denied != null) return denied;

            if (files == null || files.Count != 2)
                return BadRequest("You must upload .mat and .hea files");

            var hasMat = files.Any(f => f.FileName.EndsWith(".mat"));
            var hasHea = files.Any(f => f.FileName.EndsWith(".hea"));
            if (!hasMat || !hasHea)
                return BadRequest("Both .mat and .hea files are required");

            try
            {
                using var form = new MultipartFormDataContent();
                foreach (var file in files)
                {
                    var content = new StreamContent(file.OpenReadStream());
                    form.Add(content, "files", file.FileName);
                }

                var response = await _httpClient.PostAsync(
                    "https://manar30-ecg-af-detection.hf.space/predict_file", form);

                var json = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, json);

                var aiResult = JsonSerializer.Deserialize<AiResponseDto>(json);
                if (aiResult == null) return BadRequest("Invalid AI response");

                _context.EcgResults.Add(new EcgResult
                {
                    CaseId = caseId,
                    FileName = files.First().FileName,
                    Result = aiResult.label,
                    Confidence = aiResult.confidence_score
                });
                await _context.SaveChangesAsync();

                return Ok(new { result = aiResult.label, confidence = aiResult.confidence_score });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "ECG analysis failed." });
            }
        }

        [HttpPost("predict-image")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<IActionResult> PredictImage([FromForm] int caseId, IFormFile file)
        {
            var denied = await AuthorizeCaseAsync(caseId);
            if (denied != null) return denied;

            if (file == null || file.Length == 0)
                return BadRequest("Image is required");

            try
            {
                using var form = new MultipartFormDataContent();
                form.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);

                var response = await _httpClient.PostAsync(
                    "https://manar30-ecg-image.hf.space/predict", form);

                var json = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, json);

                var aiResult = JsonSerializer.Deserialize<EcgImageResponseDto>(json);
                if (aiResult == null) return BadRequest("Invalid AI response");

                _context.EcgResults.Add(new EcgResult
                {
                    CaseId = caseId,
                    FileName = file.FileName,
                    Result = aiResult.ClassName,
                    Confidence = aiResult.Confidence
                });
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Class = aiResult.ClassName,
                    Confidence = aiResult.Confidence,
                    Status = aiResult.Status
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "ECG image analysis failed." });
            }
        }

        [HttpPost("predict-stroke")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<IActionResult> PredictStroke([FromForm] int caseId, List<IFormFile> files)
        {
            var denied = await AuthorizeCaseAsync(caseId);
            if (denied != null) return denied;

            if (files == null || files.Count == 0)
                return BadRequest("At least one image is required");

            try
            {
                using var form = new MultipartFormDataContent();
                foreach (var file in files)
                {
                    form.Add(new StreamContent(file.OpenReadStream()), "files", file.FileName);
                }

                var response = await _httpClient.PostAsync(
                    "https://manar30-stroke.hf.space/predict_patient", form);

                var json = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, json);

                var aiResult = JsonSerializer.Deserialize<StrokeResponseDto>(json);
                if (aiResult == null) return BadRequest("Invalid AI response");

                _context.StrokeResults.Add(new StrokeResult
                {
                    CaseId = caseId,
                    FileName = files.First().FileName,
                    Diagnosis = aiResult.PatientFinalDiagnosis,
                    Confidence = aiResult.Confidence,
                    TotalImagesProcessed = aiResult.TotalImagesProcessed
                });
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    total_images_processed = aiResult.TotalImagesProcessed,
                    diagnosis = aiResult.PatientFinalDiagnosis,
                    confidence = aiResult.Confidence,
                    status = aiResult.Status
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Stroke prediction failed." });
            }
        }

        [HttpGet("case/{caseId}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<IActionResult> GetByCaseId(int caseId)
        {
            var denied = await AuthorizeCaseAsync(caseId);
            if (denied != null) return denied;

            var results = await _context.EcgResults
                .Where(e => e.CaseId == caseId)
                .Select(e => new { e.Id, e.CaseId, e.FileName, e.Result, e.Confidence, e.CreatedAt })
                .ToListAsync();

            return Ok(results);
        }

        [HttpGet("stroke/case/{caseId}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<IActionResult> GetStrokeByCaseId(int caseId)
        {
            var denied = await AuthorizeCaseAsync(caseId);
            if (denied != null) return denied;

            var results = await _context.StrokeResults
                .Where(s => s.CaseId == caseId)
                .Select(s => new { s.Id, s.CaseId, s.FileName, s.Diagnosis, s.Confidence, s.TotalImagesProcessed, s.CreatedAt })
                .ToListAsync();

            return Ok(results);
        }

        private async Task<IActionResult?> AuthorizeCaseAsync(int caseId)
        {
            try
            {
                await _caseAccess.EnsureCanAccessCaseAsync(caseId);
                return null;
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
