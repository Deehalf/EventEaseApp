namespace EventEaseApp.Models
{
    public class AttendanceRecord
    {
        public int EventId { get; set; }
        public int Count { get; set; }
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    }
}