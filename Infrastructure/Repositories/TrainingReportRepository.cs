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
                    .ThenInclude(assignment => assignment.Topic)
                .FirstOrDefaultAsync(
                    report => report.Id == id,
                    cancellationToken
                );
        }

        public async Task<bool> DeleteWithDetailsAsync(int id, CancellationToken cancellationToken = default)
        {
            var report = await _context
                .Set<TrainingReport>()
                .AsSplitQuery()
                .Include(report => report.WeldingUnionTypes)
                .Include(report => report.Attendees)
                    .ThenInclude(attendee => attendee.Topics)
                .FirstOrDefaultAsync(
                    report => report.Id == id,
                    cancellationToken
                );

            if( report is null )
            {
                return false;
            }


            foreach (var attendee in report.Attendees)
            {
                attendee.Topics.Clear();
            }

            if (report.WeldingUnionTypes.Count > 0)
            {
                _context
                    .Set<WeldingReportUnionType>()
                    .RemoveRange(report.WeldingUnionTypes);
            }

            if (report.Attendees.Count > 0)
            {
                _context
                    .Set<TrainingReportAttendee>()
                    .RemoveRange(report.Attendees);
            }

            _context
                .Set<TrainingReport>()
                .Remove(report);

            return true;
        }

        public async Task<TrainingReport?> GetTrackedDetailsByIdAsync(
            int id,
            CancellationToken cancellationToken = default
            )
        {
            return await _context
                .Set<TrainingReport>()
                .AsSplitQuery()
                .Include(report => report.WeldingUnionTypes)
                .Include(report => report.Attendees)
                    .ThenInclude(attendee => attendee.Topics)
                .FirstOrDefaultAsync(
                    report => report.Id == id,
                    cancellationToken
                );
        }

        public void DeleteAttendees(IEnumerable<TrainingReportAttendee> attendees)
        {
            _context
                .Set<TrainingReportAttendee>()
                .RemoveRange(attendees);
        }

        public void DeleteUnionTypes(IEnumerable<WeldingReportUnionType> unionTypes)
        {
            _context
                .Set<WeldingReportUnionType>()
                .RemoveRange(unionTypes);
        }

    }
}
