using RivenBackend.DTOs;
using RivenBackend.Models;

namespace RivenBackend.Mappings
{
    public static class AttachmentMapper
    {
        public static AttachmentDto ToDto(Attachment attachment) => new()
        {
            AttachmentId = attachment.AttachmentId,
            CaseId = attachment.CaseId,
            FileUrl = attachment.FileUrl,
            Type = attachment.Type,
            FileName = attachment.FileName,
            FileSize = attachment.FileSize,
            UploadedAt = attachment.UploadedAt
        };

        public static IEnumerable<AttachmentDto> ToDtoList(IEnumerable<Attachment> attachments) =>
            attachments.Select(ToDto);
    }
}
