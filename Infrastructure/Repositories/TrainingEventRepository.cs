using Domain.Entities;
using Domain.Interfaces;
using Domain.Common;
using Domain.ReadModels;
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
                    .ThenInclude(a => a.ProductionLine)
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

        public async Task<PagedResult<TrainingEventSummary>> GetAllEventsSummaryAsync(
            int page,
            int pageSize,
            string? search,
            string? status,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<TrainingEvent>().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim();
                query = query.Where(e => e.CourseName.Contains(searchTerm)
                    || e.InstructorName.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                var statusFilter = status.Trim();
                query = query.Where(e => e.Status == statusFilter);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(e => e.CreatedAt)
                .ThenByDescending(e => e.Id)
                .Skip(checked((page - 1) * pageSize))
                .Take(pageSize)
                .Select(e => new TrainingEventSummary
                {
                    Id = e.Id,
                    CourseName = e.CourseName,
                    InstructorName = e.InstructorName,
                    DateFrom = e.DateFrom,
                    DateTo = e.DateTo,
                    Status = e.Status,
                    AttendeeCount = e.Attendees.Count(),
                    TopicCount = e.Topics.Count()
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<TrainingEventSummary>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
