namespace RivenBackend.DTOs
{
    public class AttachmentDto
    {
        public int AttachmentId { get; set; }
        public int CaseId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public long? FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class CreateAttachmentDto
    {
        public int CaseId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? FileName { get; set; }
        public long? FileSize { get; set; }
    }
}