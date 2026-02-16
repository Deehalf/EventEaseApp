using System.Collections.Generic;
using System.Linq;
using EventEaseApp.Models;

namespace EventEaseApp.Services
{
    public class EventService
    {
        private readonly List<Event> _events = new()
        {
            new Event { Id = 1, Name = "Concierto de Verano", Date = DateTime.Now.AddDays(10), Location = "Parque Central", Description = "Música en vivo" },
            new Event { Id = 2, Name = "Feria Gastronómica", Date = DateTime.Now.AddDays(20), Location = "Plaza Mayor", Description = "Sabores locales" }
        };
        private int _nextId = 3;

        public IEnumerable<Event> GetAll() => _events.OrderBy(e => e.Date);
        public Event? GetById(int id) => _events.FirstOrDefault(e => e.Id == id);
        public Event Add(Event ev)
        {
            ev.Id = _nextId++;
            _events.Add(ev);
            return ev;
        }

        // Nuevo: eliminar evento por id
        public bool Remove(int id)
        {
            var ev = GetById(id);
            if (ev is null) return false;
            return _events.Remove(ev);
        }
    }
}