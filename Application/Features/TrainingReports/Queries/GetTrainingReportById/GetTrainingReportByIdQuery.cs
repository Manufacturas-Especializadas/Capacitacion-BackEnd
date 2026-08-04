using Application.DTOs.TrainingReports;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.TrainingReports.Queries.GetTrainingReportById
{
    public record GetTrainingReportByIdQuery(int Id) : IRequest<TrainingReportDetailsDto?>;
}
