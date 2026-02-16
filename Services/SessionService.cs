using System.Collections.Concurrent;
using EventEaseApp.Models;

namespace EventEaseApp.Services
{
    public class SessionService
    {
        private readonly ConcurrentDictionary<Guid, UserSession> _sessions = new();

        public UserSession StartSession(string userName)
        {
            var s = new UserSession { UserName = userName };
            _sessions[s.SessionId] = s;
            return s;
        }

        public bool EndSession(Guid sessionId)
        {
            if (_sessions.TryGetValue(sessionId, out var s) && s.IsActive)
            {
                s.EndedAt = DateTime.UtcNow;
                return true;
            }
            return false;
        }

        public IEnumerable<UserSession> GetActiveSessions() =>
            _sessions.Values.Where(x => x.IsActive).OrderByDescending(x => x.StartedAt);

        public IEnumerable<UserSession> GetAllSessions() => _sessions.Values.OrderByDescending(x => x.StartedAt);
    }
}