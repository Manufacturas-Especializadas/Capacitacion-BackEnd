using Domain.Entities;
using Domain.Common;
using Domain.ReadModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface ITrainingReportRepository
        : IGenericRepository<TrainingReport>
    {
        Task<TrainingReport?> GetDetailsByIdAsync(
            int id, 
            CancellationToken cancellationToken = default 
        );

        Task<TrainingReport?> GetTrackedDetailsByIdAsync(
            int id,
            CancellationToken cancellationToken = default
        );

        Task<bool> DeleteWithDetailsAsync(
            int id, 
            CancellationToken cancellationToken = default
        );

        Task<PagedResult<TrainingReportSummary>> GetAllReportsSummaryAsync(
            int page,
            int pageSize,
            string? search,
            string? trainingType,
            int? weekNumber,
            CancellationToken cancellationToken = default
        );

        void DeleteAttendees(
            IEnumerable<TrainingReportAttendee> attendees
        );

        void DeleteUnionTypes(
            IEnumerable<WeldingReportUnionType> unionTypes
        );

    }
}
