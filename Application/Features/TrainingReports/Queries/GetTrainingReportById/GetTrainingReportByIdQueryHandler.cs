using Application.DTOs.TrainingReports;
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.TrainingReports.Queries.GetTrainingReportById
{
    public class GetTrainingReportByIdQueryHandler(
        IUnitOfWork unitOfWork
    ) : IRequestHandler<
        GetTrainingReportByIdQuery,
        TrainingReportDetailsDto?
    >
    {
        public async Task<TrainingReportDetailsDto?> Handle(
            GetTrainingReportByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var report =
                await unitOfWork.TrainingReports.GetDetailsByIdAsync(
                    request.Id,
                    cancellationToken
                );

            if (report is null)
            {
                return null;
            }

            return new TrainingReportDetailsDto
            {
                Id = report.Id,
                TrainingType = report.TrainingType,
                LeaderName = report.LeaderName,
                LeaderPayroll = report.LeaderPayroll,
                WeekNumber = report.WeekNumber,
                Observations = report.Observations,
                InstructorSignatureUrl = report.InstructorSignatureUrl,
                CoordinatorSignatureUrl = report.CoordinatorSignatureUrl,
                SecuritySignatureUrl = report.SecuritySignatureUrl,
                CreatedAt = report.CreatedAt,

                WeldingUnionTypes = report.WeldingUnionTypes
                    .Select(union => new WeldingReportUnionTypeDetailsDto
                    {
                        Id = union.Id,
                        ListNumber = union.ListNumber,
                        UnionName = union.UnionName
                    })
                    .ToList(),

                Attendees = report.Attendees
                    .Select(attendee =>
                        new TrainingReportAttendeeDetailsDto
                        {
                            Id = attendee.Id,
                            EmployeeId = attendee.EmployeeId,

                            EmployeeNumber =
                                attendee.Employee?.EmployeeNumber
                                ?? "Sin nómina",

                            EmployeeName =
                                attendee.Employee?.Name
                                ?? "Empleado no encontrado",

                            LineId = attendee.LineId,

                            LineName =
                                attendee.ProductionLine?.LineName
                                ?? attendee.Employee?
                                    .ProductionLine?
                                    .LineName
                                ?? "Sin línea",

                            DayMonday = attendee.DayMonday,
                            DayTuesday = attendee.DayTuesday,
                            DayWednesday = attendee.DayWednesday,
                            DayThursday = attendee.DayThursday,
                            DayFriday = attendee.DayFriday,
                            DaySaturday = attendee.DaySaturday,
                            DaySunday = attendee.DaySunday,

                            CustomerClient = attendee.CustomerClient,
                            UnionClassification =
                                attendee.UnionClassification,
                            WeldingPercentage =
                                attendee.WeldingPercentage,
                            Diameter = attendee.Diameter,
                            Shift = attendee.Shift,
                            Machinery = attendee.Machinery,
                            Ast = attendee.Ast,

                            TraineeSignatureUrl =
                                attendee.TraineeSignatureUrl,

                            SupervisorSignatureUrl =
                                attendee.SupervisorSignatureUrl,

                            Topics = attendee.Topics
                                .Select(topic =>
                                    new TrainingReportTopicDetailsDto
                                    {
                                        Id = topic.Id,
                                        TrainingType =
                                            topic.TrainingType,
                                        TopicCode =
                                            topic.TopicCode,
                                        TopicName =
                                            topic.TopicName
                                    })
                                .ToList()
                        })
                    .ToList()
            };
        }
    }
}
