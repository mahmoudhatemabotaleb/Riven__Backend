namespace RivenBackend.DTOs
{
    public class AuditLogDto
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
    }

    public class CreateAuditLogDto
    {
        public int UserId { get; set; }
        public string EntityName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
    }
}