using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.TrainingReports.Commands
{
    public enum UpdateTrainingReportStatus
    {
        Updated,
        NotFound,
        InvalidRequest
    }

    public record UpdateTrainingReportResult(
        UpdateTrainingReportStatus Status,
        string Message
    );
}
