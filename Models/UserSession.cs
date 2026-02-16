namespace EventEaseApp.Models
{
    public class UserSession
    {
        public Guid SessionId { get; set; } = Guid.NewGuid();
        public string UserName { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }
        public bool IsActive => EndedAt == null;
    }
}