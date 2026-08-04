using Application.DTOs.TrainingReports;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.TrainingReports.Queries.GetAllTrainingReports
{
    public record GetAllTrainingReportsQuery: IRequest<IReadOnlyList<TrainingReportSummaryDto>>;
}
