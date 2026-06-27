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
        private readonly ILogger<EcgController> _logger;
        private const int HUGGING_FACE_TIMEOUT_SECONDS = 180; // 3 minutes for HF API

        public EcgController(
            IHttpClientFactory httpClientFactory,
            AppDbContext context,
            ICaseAccessService caseAccess,
            ILogger<EcgController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _context = context;
            _caseAccess = caseAccess;
            _logger = logger;
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
                _logger.LogInformation($"[ECG-ANALYZE] Starting analysis for caseId: {caseId}");

                using var form = new MultipartFormDataContent();
                foreach (var file in files)
                {
                    var content = new StreamContent(file.OpenReadStream());
                    form.Add(content, "files", file.FileName);
                }

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(HUGGING_FACE_TIMEOUT_SECONDS));
                var response = await _httpClient.PostAsync(
                    "https://manar30-ecg-af-detection.hf.space/predict_file", form, cts.Token);

                var json = await response.Content.ReadAsStringAsync(cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"[ECG-ANALYZE] HF API failed with status {response.StatusCode}: {json}");
                    return StatusCode((int)response.StatusCode, new { error = "Hugging Face API failed", details = json });
                }

                var aiResult = JsonSerializer.Deserialize<AiResponseDto>(json);
                if (aiResult == null)
                {
                    _logger.LogError($"[ECG-ANALYZE] Failed to deserialize response: {json}");
                    return BadRequest("Invalid AI response format");
                }

                _context.EcgResults.Add(new EcgResult
                {
                    CaseId = caseId,
                    FileName = files.First().FileName,
                    Result = aiResult.label,
                    Confidence = aiResult.confidence_score
                });
                await _context.SaveChangesAsync();

                _logger.LogInformation($"[ECG-ANALYZE] Success - Result: {aiResult.label}, Confidence: {aiResult.confidence_score}");
                return Ok(new { result = aiResult.label, confidence = aiResult.confidence_score });
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError($"[ECG-ANALYZE] Timeout after {HUGGING_FACE_TIMEOUT_SECONDS}s: {ex.Message}");
                return StatusCode(504, new { error = "ECG analysis timeout", message = "Hugging Face service is taking too long" });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"[ECG-ANALYZE] Connection error: {ex.Message}");
                return StatusCode(503, new { error = "Connection failed", message = "Cannot reach Hugging Face API" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ECG-ANALYZE] Unexpected error: {ex}");
                return StatusCode(500, new { error = "ECG analysis failed", message = ex.Message });
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
                _logger.LogInformation($"[ECG-IMAGE] Starting prediction for caseId: {caseId}, file: {file.FileName}");

                using var form = new MultipartFormDataContent();
                form.Add(new StreamContent(file.OpenReadStream()), "file", file.FileName);

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(HUGGING_FACE_TIMEOUT_SECONDS));
                var response = await _httpClient.PostAsync(
                    "https://manar30-ecg-image.hf.space/predict", form, cts.Token);

                var json = await response.Content.ReadAsStringAsync(cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"[ECG-IMAGE] HF API failed with status {response.StatusCode}: {json}");
                    return StatusCode((int)response.StatusCode, new { error = "Hugging Face API failed", details = json });
                }

                var aiResult = JsonSerializer.Deserialize<EcgImageResponseDto>(json);
                if (aiResult == null)
                {
                    _logger.LogError($"[ECG-IMAGE] Failed to deserialize response: {json}");
                    return BadRequest("Invalid AI response format");
                }

                _context.EcgResults.Add(new EcgResult
                {
                    CaseId = caseId,
                    FileName = file.FileName,
                    Result = aiResult.ClassName,
                    Confidence = aiResult.Confidence
                });
                await _context.SaveChangesAsync();

                _logger.LogInformation($"[ECG-IMAGE] Success - Class: {aiResult.ClassName}, Confidence: {aiResult.Confidence}");
                return Ok(new
                {
                    Class = aiResult.ClassName,
                    Confidence = aiResult.Confidence,
                    Status = aiResult.Status
                });
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError($"[ECG-IMAGE] Timeout after {HUGGING_FACE_TIMEOUT_SECONDS}s: {ex.Message}");
                return StatusCode(504, new { error = "Image prediction timeout", message = "Hugging Face service is taking too long" });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"[ECG-IMAGE] Connection error: {ex.Message}");
                return StatusCode(503, new { error = "Connection failed", message = "Cannot reach Hugging Face API" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ECG-IMAGE] Unexpected error: {ex}");
                return StatusCode(500, new { error = "ECG image analysis failed", message = ex.Message });
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
                _logger.LogInformation($"[STROKE] Starting prediction for caseId: {caseId}, files: {files.Count}");

                using var form = new MultipartFormDataContent();
                foreach (var file in files)
                {
                    form.Add(new StreamContent(file.OpenReadStream()), "files", file.FileName);
                }

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(HUGGING_FACE_TIMEOUT_SECONDS));
                var response = await _httpClient.PostAsync(
                    "https://manar30-stroke.hf.space/predict_patient", form, cts.Token);

                var json = await response.Content.ReadAsStringAsync(cts.Token);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"[STROKE] HF API failed with status {response.StatusCode}: {json}");
                    return StatusCode((int)response.StatusCode, new { error = "Hugging Face API failed", details = json });
                }

                var aiResult = JsonSerializer.Deserialize<StrokeResponseDto>(json);
                if (aiResult == null)
                {
                    _logger.LogError($"[STROKE] Failed to deserialize response: {json}");
                    return BadRequest("Invalid AI response format");
                }

                _context.StrokeResults.Add(new StrokeResult
                {
                    CaseId = caseId,
                    FileName = files.First().FileName,
                    Diagnosis = aiResult.PatientFinalDiagnosis,
                    Confidence = aiResult.Confidence,
                    TotalImagesProcessed = aiResult.TotalImagesProcessed
                });
                await _context.SaveChangesAsync();

                _logger.LogInformation($"[STROKE] Success - Diagnosis: {aiResult.PatientFinalDiagnosis}, Confidence: {aiResult.Confidence}");
                return Ok(new
                {
                    total_images_processed = aiResult.TotalImagesProcessed,
                    diagnosis = aiResult.PatientFinalDiagnosis,
                    confidence = aiResult.Confidence,
                    status = aiResult.Status
                });
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError($"[STROKE] Timeout after {HUGGING_FACE_TIMEOUT_SECONDS}s: {ex.Message}");
                return StatusCode(504, new { error = "Stroke prediction timeout", message = "Hugging Face service is taking too long" });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"[STROKE] Connection error: {ex.Message}");
                return StatusCode(503, new { error = "Connection failed", message = "Cannot reach Hugging Face API" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"[STROKE] Unexpected error: {ex}");
                return StatusCode(500, new { error = "Stroke prediction failed", message = ex.Message });
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
                _logger.LogWarning($"[ECG] Unauthorized access attempt for caseId: {caseId}");
                return Forbid();
            }
        }
    }
}