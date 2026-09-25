using Application.DTOs.TrainingReports;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.TrainingReports.Queries.GetAllTrainingReports
{
    public class GetAllTrainingReportsQueryHandler(IUnitOfWork unitOfWork) : 
        IRequestHandler<GetAllTrainingReportsQuery,
        IReadOnlyList<TrainingReportSummaryDto>>
    {
        public async Task<
            IReadOnlyList<TrainingReportSummaryDto>
        > Handle(
            GetAllTrainingReportsQuery request,
            CancellationToken cancellationToken
        )
        {
            var reports = await unitOfWork
                .TrainingReports
                .GetAllWithAttendeesAsync(cancellationToken);

            return reports
                .Select(report => new TrainingReportSummaryDto
                {
                    Id = report.Id,
                    TrainingType = report.TrainingType,
                    LeaderName = report.LeaderName,
                    WeekNumber = report.WeekNumber,
                    AttendeesCount = report.Attendees.Count,

                    TotalTrainingMinutes =
                report.Attendees.Sum(
                    attendee =>
                        ConvertHourMinuteToMinutes(
                            attendee.TotalHours
                        )
                ),
                    CreatedAt = report.CreatedAt
                })
                .ToList();
        }

        private static int ConvertHourMinuteToMinutes(
    decimal? value
)
        {
            if (!value.HasValue)
            {
                return 0;
            }

            var hours =
                decimal.ToInt32(
                    decimal.Truncate(
                        value.Value
                    )
                );

            var minutes =
                decimal.ToInt32(
                    (
                        value.Value -
                        decimal.Truncate(
                            value.Value
                        )
                    ) * 100m
                );

            return
                (hours * 60) +
                minutes;
        }

    }
}
