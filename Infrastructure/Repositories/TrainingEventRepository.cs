using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class TrainingEventRepository(ApplicationDbContext context) : GenericRepository<TrainingEvent>(context), ITrainingEventRepository
    {
        public async Task<TrainingEvent?> GetEventWithDetailAsync(int eventId)
        {
            return await _context.TrainingEvents
                .Include(e => e.Room)
                .Include(e => e.Topics.OrderBy(t => t.TrainingEvent))
                .Include(e => e.Attendees)
                    .ThenInclude(a => a.Employee)
                .Include(e => e.Attendees)
                    .ThenInclude(a => a.Evaluations)
                .FirstOrDefaultAsync(e => e.Id == eventId);
        }

        public async Task<IReadOnlyList<TrainingEvent>> GetActiveEventsAsync()
        {
            return await _context.TrainingEvents
                    .Where(e => e.Status != "Completado")
                    .OrderBy(e => e.DateFrom)
                    .AsNoTracking()
                    .ToListAsync();
        }

        public async Task<TrainingEvent?> GetEventWithAttendeesAsync(int id)
        {
            return await _context.Set<TrainingEvent>()
                    .Include(t => t.Topics)
                    .Include(t => t.Attendees)
                        .ThenInclude(a => a.Evaluations)
                    .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}