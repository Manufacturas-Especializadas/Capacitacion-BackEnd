using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class TrainingReportRepository(ApplicationDbContext context) : GenericRepository<TrainingReport>(context),ITrainingReportRepository
    {
        public async Task<IReadOnlyList<TrainingReport>>GetAllWithAttendeesAsync(CancellationToken cancellationToken = default)
        {
            return await _context
                .Set<TrainingReport>()
                .AsNoTracking()
                .Include(report => report.Attendees)
                .OrderByDescending(report => report.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        public async Task<TrainingReport?> GetDetailsByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context
                .Set<TrainingReport>()
                .AsNoTracking()
                .AsSplitQuery()
                .Include(report => report.WeldingUnionTypes)
                .Include(report => report.Attendees)
                    .ThenInclude(attendee => attendee.Employee)
                .Include(report => report.Attendees)
                    .ThenInclude(attendee => attendee.ProductionLine)
                .Include(report => report.Attendees)
                    .ThenInclude(attendee => attendee.Topics)
                .FirstOrDefaultAsync(
                    report => report.Id == id,
                    cancellationToken
                );
        }
    }
}
