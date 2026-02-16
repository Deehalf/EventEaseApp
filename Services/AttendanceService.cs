using System.Collections.Concurrent;
using EventEaseApp.Models;

namespace EventEaseApp.Services
{
    public class AttendanceService
    {
        private readonly ConcurrentDictionary<int, AttendanceRecord> _attendance = new();

        public void SetAttendance(int eventId, int count)
        {
            _attendance[eventId] = new AttendanceRecord { EventId = eventId, Count = count, RecordedAt = DateTime.UtcNow };
        }

        public int GetAttendance(int eventId) =>
            _attendance.TryGetValue(eventId, out var r) ? r.Count : 0;

        public AttendanceRecord? GetRecord(int eventId) =>
            _attendance.TryGetValue(eventId, out var r) ? r : null;
    }
}