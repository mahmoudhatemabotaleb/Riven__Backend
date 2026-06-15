namespace RivenBackend.Security
{
    public class FileUploadValidator
    {
        private readonly HashSet<string> _allowedExtensions;
        private readonly long _maxFileSizeBytes;

        public FileUploadValidator(IConfiguration configuration)
        {
            var extensions = configuration.GetSection("Security:AllowedFileExtensions").Get<string[]>()
                ?? [".jpg", ".jpeg", ".png", ".webp", ".pdf", ".dcm", ".csv", ".txt"];
            _allowedExtensions = extensions.Select(e => e.ToLowerInvariant()).ToHashSet();

            var maxMb = configuration.GetValue("Security:MaxUploadSizeMb", 10);
            _maxFileSizeBytes = maxMb * 1024L * 1024L;
        }

        public bool TryValidate(IFormFile file, out string error)
        {
            if (file == null || file.Length == 0)
            {
                error = "File is required.";
                return false;
            }

            if (file.Length > _maxFileSizeBytes)
            {
                error = $"File exceeds maximum size of {_maxFileSizeBytes / (1024 * 1024)} MB.";
                return false;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !_allowedExtensions.Contains(extension))
            {
                error = $"File type '{extension}' is not allowed.";
                return false;
            }

            if (file.FileName.Contains("..") || file.FileName.Contains('/') || file.FileName.Contains('\\'))
            {
                error = "Invalid file name.";
                return false;
            }

            error = string.Empty;
            return true;
        }
    }
}
