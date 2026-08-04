using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Interfaces
{
    public interface ITrainingReportRepository
        : IGenericRepository<TrainingReport>
    {
        Task<TrainingReport?> GetDetailsByIdAsync(int id, CancellationToken cancellationToken = default );
        Task<IReadOnlyList<TrainingReport>> GetAllWithAttendeesAsync(CancellationToken cancellationToken = default );
    }
}
