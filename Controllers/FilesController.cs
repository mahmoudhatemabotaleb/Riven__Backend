using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RivenBackend.Models;
using RivenBackend.Security;
using RivenBackend.Services;

namespace RivenBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IFileStorageService _fileStorage;
        private readonly ICurrentUserService _currentUser;

        public FilesController(IFileStorageService fileStorage, ICurrentUserService currentUser)
        {
            _fileStorage = fileStorage;
            _currentUser = currentUser;
        }

        // GET: api/files/{attachmentId}
        [HttpGet("{attachmentId:int}")]
        [Authorize(Roles = "Admin,Doctor,Paramedic")]
        public async Task<IActionResult> Download(int attachmentId)
        {
            var result = await _fileStorage.OpenAsync(
                attachmentId,
                _currentUser.UserId,
                _currentUser.Role,
                _currentUser.HospitalId);

            if (result == null)
                return NotFound(new ApiResponse { Success = false, Message = "File not found or access denied." });

            return File(result.Value.Stream, result.Value.ContentType, result.Value.FileName);
        }
    }
}
