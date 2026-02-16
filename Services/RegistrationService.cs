using System.Collections.Generic;
using System.Linq;
using EventEaseApp.Models;

namespace EventEaseApp.Services
{
    public class RegistrationService
    {
        private readonly List<Registration> _regs = new();
        private int _nextId = 1;

        public IEnumerable<Registration> GetAll() => _regs;
        public IEnumerable<Registration> GetByEventId(int eventId) => _regs.Where(r => r.EventId == eventId);
        public Registration Add(Registration reg)
        {
            reg.Id = _nextId++;
            _regs.Add(reg);
            return reg;
        }
        public bool Remove(int id) => _regs.RemoveAll(r => r.Id == id) > 0;
    }
}