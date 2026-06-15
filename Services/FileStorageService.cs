using RivenBackend.Models;
using RivenBackend.Repositories;
using RivenBackend.Security;

namespace RivenBackend.Services
{
    public interface IFileStorageService
    {
        Task<Attachment> SaveAsync(int caseId, IFormFile file, string type);
        Task<(Stream Stream, string ContentType, string FileName)?> OpenAsync(int attachmentId, int userId, string role, int hospitalId);
    }

    public class FileStorageService : IFileStorageService
    {
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly ICaseRepository _caseRepository;
        private readonly IWebHostEnvironment _env;
        private readonly FileUploadValidator _validator;

        public FileStorageService(
            IAttachmentRepository attachmentRepository,
            ICaseRepository caseRepository,
            IWebHostEnvironment env,
            FileUploadValidator validator)
        {
            _attachmentRepository = attachmentRepository;
            _caseRepository = caseRepository;
            _env = env;
            _validator = validator;
        }

        public async Task<Attachment> SaveAsync(int caseId, IFormFile file, string type)
        {
            if (!_validator.TryValidate(file, out var error))
                throw new InvalidOperationException(error);

            var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);

            var safeName = Path.GetFileName(file.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}_{safeName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new Attachment
            {
                CaseId = caseId,
                FileUrl = uniqueFileName,
                Type = type,
                FileName = safeName,
                FileSize = file.Length,
                UploadedAt = DateTime.UtcNow
            };

            _attachmentRepository.Add(attachment);
            return attachment;
        }

        public async Task<(Stream Stream, string ContentType, string FileName)?> OpenAsync(
            int attachmentId, int userId, string role, int hospitalId)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(attachmentId);
            if (attachment == null) return null;

            var case_ = await _caseRepository.GetByIdAsync(attachment.CaseId);
            if (case_ == null) return null;

            var canAccess = role == "Admin"
                || (role == "Doctor" && case_.HospitalId == hospitalId)
                || (role == "Paramedic" && case_.UserId == userId);

            if (!canAccess) return null;

            var storedName = attachment.FileUrl.StartsWith("/uploads/")
                ? Path.GetFileName(attachment.FileUrl)
                : attachment.FileUrl;

            var filePath = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", storedName);
            if (!File.Exists(filePath)) return null;

            var contentType = GetContentType(attachment.FileName ?? "file.bin");
            return (new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read), contentType, attachment.FileName ?? "file.bin");
        }

        private static string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                ".dcm" => "application/dicom",
                ".csv" => "text/csv",
                _ => "application/octet-stream"
            };
        }
    }
}
