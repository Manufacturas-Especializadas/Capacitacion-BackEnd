using Domain.Entities;
using Domain.Interfaces;
using Domain.Common;
using Domain.ReadModels;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Repositories
{
    public class TrainingReportRepository(ApplicationDbContext context) : GenericRepository<TrainingReport>(context),ITrainingReportRepository
    {
        public async Task<PagedResult<TrainingReportSummary>> GetAllReportsSummaryAsync(
            int page,
            int pageSize,
            string? search,
            string? trainingType,
            int? weekNumber,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Set<TrainingReport>().AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim();
                if (int.TryParse(searchTerm, out var reportId))
                {
                    query = query.Where(report => report.LeaderName.Contains(searchTerm)
                        || report.Id == reportId);
                }
                else
                {
                    query = query.Where(report => report.LeaderName.Contains(searchTerm));
                }
            }

            if (!string.IsNullOrWhiteSpace(trainingType))
            {
                var trainingTypeFilter = trainingType.Trim();
                query = query.Where(report => report.TrainingType == trainingTypeFilter);
            }

            if (weekNumber.HasValue)
            {
                query = query.Where(report => report.WeekNumber == weekNumber.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(report => report.CreatedAt)
                .ThenByDescending(report => report.Id)
                .Skip(checked((page - 1) * pageSize))
                .Take(pageSize)
                .Select(report => new TrainingReportSummary
                {
                    Id = report.Id,
                    TrainingType = report.TrainingType,
                    LeaderName = report.LeaderName,
                    WeekNumber = report.WeekNumber,
                    AttendeesCount = report.Attendees.Count(),
                    // TotalHours encodes HH.MM, not decimal hours. Integer casts
                    // truncate toward zero in both C# and SQL Server.
                    TotalTrainingMinutes = report.Attendees.Sum(attendee =>
                        (int)(attendee.TotalHours ?? 0m) * 60
                        + (int)(((attendee.TotalHours ?? 0m)
                            - (int)(attendee.TotalHours ?? 0m)) * 100m)),
                    CreatedAt = report.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<TrainingReportSummary>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
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
