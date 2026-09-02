using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.TrainingReports.Commands
{


    public class CreateWeldingUnionTypeDto
    {
        public int ListNumber { get; set; }

        public required string UnionName { get; set; }
    }



    public class CreateTrainingReportAttendeeTopicDto
    {
        public int TopicId { get; set; }
    }



    public class CreateTrainingReportAttendeeDto
    {
        public int EmployeeId { get; set; }

        public int LineId { get; set; }


        public bool DayMonday { get; set; }

        public bool DayTuesday { get; set; }

        public bool DayWednesday { get; set; }

        public bool DayThursday { get; set; }

        public bool DayFriday { get; set; }

        public bool DaySaturday { get; set; }

        public bool DaySunday { get; set; }

        public decimal? HoursMonday { get; set; }

        public decimal? HoursTuesday { get; set; }

        public decimal? HoursWednesday { get; set; }

        public decimal? HoursThursday { get; set; }

        public decimal? HoursFriday { get; set; }

        public decimal? HoursSaturday { get; set; }

        public decimal? HoursSunday { get; set; }

        public string? CustomerClient { get; set; }

        public string? UnionClassification { get; set; }

        public string? WeldingPercentage { get; set; }

        public string? Diameter { get; set; }

        public string? Shift { get; set; }

        public string? Machinery { get; set; }

        public string? Ast { get; set; }

        public List<int> TopicIds { get; set; } = new();


        public List<CreateTrainingReportAttendeeTopicDto>
            Topics
        { get; set; } = new();



        public IFormFile? TraineeSignature { get; set; }

        public IFormFile? SupervisorSignature { get; set; }
    }


    public enum CreateTrainingReportStatus
    {
        Created,
        InvalidRequest
    }


    public sealed record CreateTrainingReportResult(
        CreateTrainingReportStatus Status,
        int? ReportId,
        string Message
    );

    public class CreateTrainingReportCommand
        : IRequest<CreateTrainingReportResult>
    {
        public required string TrainingType { get; set; }

        public required string LeaderName { get; set; }

        public required string LeaderPayroll { get; set; }

        public int? WeekNumber { get; set; }

        public string? Observations { get; set; }



        public IFormFile? InstructorSignature { get; set; }

        public IFormFile? CoordinatorSignature { get; set; }

        public IFormFile? SecuritySignature { get; set; }



        public List<CreateWeldingUnionTypeDto>?
            UnionTypes
        { get; set; } = new();



        public required List<CreateTrainingReportAttendeeDto>
            Attendees
        { get; set; } = new();
    }



    public class CreateTrainingReportCommandHandler(
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorage
    ) : IRequestHandler<
        CreateTrainingReportCommand,
        CreateTrainingReportResult
    >
    {

        public async Task<CreateTrainingReportResult> Handle(
            CreateTrainingReportCommand request,
            CancellationToken cancellationToken
        )
        {

            var requestError =
                ValidateRequest(request);

            if (requestError is not null)
            {
                return Invalid(requestError);
            }


            var topicsResult =
                await LoadTopicsAsync(
                    request.Attendees,
                    cancellationToken
                );

            if (topicsResult.Error is not null)
            {
                return Invalid(
                    topicsResult.Error
                );
            }



            var newBlobUrls =
                new List<string>();


            try
            {


                var mexicoTimeZone =
                    TimeZoneInfo.FindSystemTimeZoneById(
                        "Central Standard Time (Mexico)"
                    );

                var nowInMexico =
                    TimeZoneInfo.ConvertTime(
                        DateTime.UtcNow,
                        mexicoTimeZone
                    );


  

                var report =
                    new TrainingReport
                    {
                        TrainingType =
                            request.TrainingType,

                        LeaderName =
                            request.LeaderName,

                        LeaderPayroll =
                            request.LeaderPayroll,

                        WeekNumber =
                            request.WeekNumber,

                        Observations =
                            request.Observations,

                        CreatedAt =
                            nowInMexico,


                        InstructorSignatureUrl =
                            await UploadAndTrackAsync(
                                request.InstructorSignature,
                                "instructor_sig.png",
                                newBlobUrls
                            ),


                        CoordinatorSignatureUrl =
                            await UploadAndTrackAsync(
                                request.CoordinatorSignature,
                                "coordinator_sig.png",
                                newBlobUrls
                            ),


                        SecuritySignatureUrl =
                            await UploadAndTrackAsync(
                                request.SecuritySignature,
                                "security_sig.png",
                                newBlobUrls
                            )
                    };


                if (
                    request.UnionTypes is not null &&
                    request.UnionTypes.Any()
                )
                {
                    foreach (
                        var union in request.UnionTypes
                    )
                    {
                        report.WeldingUnionTypes.Add(
                            new WeldingReportUnionType
                            {
                                ListNumber =
                                    union.ListNumber,

                                UnionName =
                                    union.UnionName
                            }
                        );
                    }
                }


                foreach (
                    var attendeeDto in request.Attendees
                )
                {

                    var topicAssignments =
                        ResolveTopicAssignments(
                            attendeeDto
                        );




                    var attendee =
                        new TrainingReportAttendee
                        {
                            EmployeeId =
                                attendeeDto.EmployeeId,

                            LineId =
                                attendeeDto.LineId,


                            DayMonday =
    attendeeDto.DayMonday,

                            DayTuesday =
    attendeeDto.DayTuesday,

                            DayWednesday =
    attendeeDto.DayWednesday,

                            DayThursday =
    attendeeDto.DayThursday,

                            DayFriday =
    attendeeDto.DayFriday,

                            DaySaturday =
    attendeeDto.DaySaturday,

                            DaySunday =
    attendeeDto.DaySunday,


                            HoursMonday =
    attendeeDto.HoursMonday,

                            HoursTuesday =
    attendeeDto.HoursTuesday,

                            HoursWednesday =
    attendeeDto.HoursWednesday,

                            HoursThursday =
    attendeeDto.HoursThursday,

                            HoursFriday =
    attendeeDto.HoursFriday,

                            HoursSaturday =
    attendeeDto.HoursSaturday,

                            HoursSunday =
    attendeeDto.HoursSunday,


                            TotalHours =
    ResolveAttendeeTotalHours(
        attendeeDto
    ),


                            CustomerClient =
                                attendeeDto.CustomerClient,

                            UnionClassification =
                                attendeeDto
                                    .UnionClassification,

                            WeldingPercentage =
                                attendeeDto
                                    .WeldingPercentage,

                            Diameter =
                                attendeeDto.Diameter,

                            Shift =
                                attendeeDto.Shift,

                            Machinery =
                                attendeeDto.Machinery,

                            Ast =
                                attendeeDto.Ast,



                            TraineeSignatureUrl =
                                await UploadAndTrackAsync(
                                    attendeeDto
                                        .TraineeSignature,

                                    $"trainee_{attendeeDto.EmployeeId}_sig.png",

                                    newBlobUrls
                                ),


                            SupervisorSignatureUrl =
                                await UploadAndTrackAsync(
                                    attendeeDto
                                        .SupervisorSignature,

                                    $"supervisor_{attendeeDto.EmployeeId}_sig.png",

                                    newBlobUrls
                                )
                        };



                    foreach (
                        var topicAssignment
                            in topicAssignments
                    )
                    {
                     
                        var topic =
                            topicsResult.Topics[
                                topicAssignment.TopicId
                            ];


                        attendee.Topics.Add(
    new TrainingReportAttendeeTopic
    {
        TopicId = topic.Id,
        Topic = topic,
        Attendee = attendee
    }
);
                    }


                    report.Attendees.Add(
                        attendee
                    );
                }



                await unitOfWork
                    .TrainingReports
                    .AddAsync(report);


                await unitOfWork
                    .SaveChangesAsync(
                        cancellationToken
                    );



                return new CreateTrainingReportResult(
                    CreateTrainingReportStatus.Created,
                    report.Id,
                    "Reporte de entrenamiento creado con éxito."
                );
            }
            catch
            {

                foreach (
                    var newBlobUrl in newBlobUrls
                )
                {
                    try
                    {
                        await blobStorage
                            .DeleteFileTrainingReportAsync(
                                newBlobUrl
                            );
                    }
                    catch
                    {
                       
                    }
                }


                throw;
            }
        }

        private static string? ValidateRequest(
            CreateTrainingReportCommand request
        )
        {
            if (
                string.IsNullOrWhiteSpace(
                    request.TrainingType
                )
            )
            {
                return
                    "El tipo de entrenamiento es obligatorio.";
            }


            if (
                string.IsNullOrWhiteSpace(
                    request.LeaderName
                )
            )
            {
                return
                    "El nombre del líder es obligatorio.";
            }


            if (
                string.IsNullOrWhiteSpace(
                    request.LeaderPayroll
                )
            )
            {
                return
                    "La nómina del líder es obligatoria.";
            }


            if (
                request.Attendees is null ||
                request.Attendees.Count == 0
            )
            {
                return
                    "Debe existir al menos un asistente.";
            }


            if (request.UnionTypes is not null)
            {
                foreach (
                    var union in request.UnionTypes
                )
                {
                    if (
                        string.IsNullOrWhiteSpace(
                            union.UnionName
                        )
                    )
                    {
                        return
                            "El nombre de la unión es obligatorio.";
                    }
                }
            }


            foreach (
                var attendee in request.Attendees
            )
            {
                if (attendee.EmployeeId <= 0)
                {
                    return
                        "La petición contiene un empleado no válido.";
                }


                if (attendee.LineId <= 0)
                {
                    return
                        "La petición contiene una línea no válida.";
                }

                var attendeeHoursError =
    ValidateAttendeeHours(attendee);

                if (attendeeHoursError is not null)
                {
                    return attendeeHoursError;
                }


                if (
                    attendee.Topics is { Count: > 0 } &&
                    attendee.Topics
                        .GroupBy(
                            topic =>
                                topic.TopicId
                        )
                        .Any(
                            group =>
                                group.Count() > 1
                        )
                )
                {
                    return
                        "Un asistente no puede tener el mismo tema asignado más de una vez.";
                }


                var topicAssignments =
                    ResolveTopicAssignments(
                        attendee
                    );


                if (topicAssignments.Count == 0)
                {
                    return
                        "Cada asistente debe tener al menos un tema.";
                }


                if (
                    topicAssignments.Any(
                        topic =>
                            topic.TopicId <= 0
                    )
                )
                {
                    return
                        "La petición contiene un tema no válido.";
                }

            }


            return null;
        }



        private static List<
            CreateTrainingReportAttendeeTopicDto
        > ResolveTopicAssignments(
            CreateTrainingReportAttendeeDto attendee
        )
        {
            
            if (
                attendee.Topics is { Count: > 0 }
            )
            {
                return attendee.Topics
                    .GroupBy(
                        topic =>
                            topic.TopicId
                    )
                    .Select(
                        group =>
                            group.First()
                    )
                    .ToList();
            }



            return attendee.TopicIds
    .Distinct()
    .Select(
        topicId =>
            new CreateTrainingReportAttendeeTopicDto
            {
                TopicId = topicId
            }
    )
    .ToList();
        }



        private async Task<TopicLoadResult>
            LoadTopicsAsync(
                IEnumerable<
                    CreateTrainingReportAttendeeDto
                > attendees,
                CancellationToken cancellationToken
            )
        {

            var topicIds =
                attendees
                    .SelectMany(
                        attendee =>
                            ResolveTopicAssignments(
                                attendee
                            )
                            .Select(
                                topic =>
                                    topic.TopicId
                            )
                    )
                    .Distinct()
                    .ToList();


            var topics =
                new Dictionary<
                    int,
                    TrainingTopic
                >();


            foreach (
                var topicId in topicIds
            )
            {
                cancellationToken
                    .ThrowIfCancellationRequested();


           
                if (topicId <= 0)
                {
                    return new TopicLoadResult(
                        topics,
                        "La petición contiene un tema no válido."
                    );
                }


                var topic =
                    await unitOfWork
                        .TrainingTopics
                        .GetByIdAsync(
                            topicId
                        );


                if (topic is null)
                {
                    return new TopicLoadResult(
                        topics,
                        $"No se encontró el tema con ID {topicId}."
                    );
                }


                topics[topic.Id] =
                    topic;
            }


            return new TopicLoadResult(
                topics,
                null
            );
        }

        private sealed record TopicLoadResult(
            Dictionary<int, TrainingTopic> Topics,
            string? Error
        );




        private async Task<string?>
            UploadAndTrackAsync(
                IFormFile? file,
                string fileName,
                ICollection<string> newBlobUrls
            )
        {
            if (
                file is null ||
                file.Length == 0
            )
            {
                return null;
            }


            var url =
                await blobStorage
                    .UploadFileTrainingReportAsync(
                        file,
                        fileName
                    );


            if (
                string.IsNullOrWhiteSpace(
                    url
                )
            )
            {
                throw new InvalidOperationException(
                    "No se pudo almacenar una firma."
                );
            }


            newBlobUrls.Add(
                url
            );


            return url;
        }


        private static CreateTrainingReportResult
            Invalid(
                string message
            )
        {
            return new CreateTrainingReportResult(
                CreateTrainingReportStatus.InvalidRequest,
                null,
                message
            );
        }
        //Metodos para actualizar
        private static bool HasAttendeeDailyHours(
    CreateTrainingReportAttendeeDto attendee
)
        {
            return
                attendee.HoursMonday.HasValue ||
                attendee.HoursTuesday.HasValue ||
                attendee.HoursWednesday.HasValue ||
                attendee.HoursThursday.HasValue ||
                attendee.HoursFriday.HasValue ||
                attendee.HoursSaturday.HasValue ||
                attendee.HoursSunday.HasValue;
        }

       private static decimal CalculateAttendeeDailyTotal(
    CreateTrainingReportAttendeeDto attendee
)
{
    var totalMinutes =
        ConvertHourMinuteToMinutes(
            attendee.HoursMonday
        ) +
        ConvertHourMinuteToMinutes(
            attendee.HoursTuesday
        ) +
        ConvertHourMinuteToMinutes(
            attendee.HoursWednesday
        ) +
        ConvertHourMinuteToMinutes(
            attendee.HoursThursday
        ) +
        ConvertHourMinuteToMinutes(
            attendee.HoursFriday
        ) +
        ConvertHourMinuteToMinutes(
            attendee.HoursSaturday
        ) +
        ConvertHourMinuteToMinutes(
            attendee.HoursSunday
        );

    return ConvertMinutesToHourMinute(
        totalMinutes
    );
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
                    decimal.Truncate(value.Value)
                );

            var minutes =
                decimal.ToInt32(
                    (
                        value.Value -
                        decimal.Truncate(value.Value)
                    ) * 100m
                );

            return (hours * 60) + minutes;
        }


        private static decimal ConvertMinutesToHourMinute(
            int totalMinutes
        )
        {
            var hours =
                totalMinutes / 60;

            var minutes =
                totalMinutes % 60;

            return
                hours +
                (minutes / 100m);
        }

        private static bool IsValidHourMinute(
    decimal value
)
        {
            if (
                value < 0m ||
                value > 8m
            )
            {
                return false;
            }

            var wholeHours =
                decimal.Truncate(value);

            var minutePart =
                (
                    value -
                    wholeHours
                ) * 100m;

            /*
             * No aceptamos fracciones menores
             * a un minuto.
             */
            if (
                minutePart !=
                decimal.Truncate(minutePart)
            )
            {
                return false;
            }

            var minutes =
                decimal.ToInt32(minutePart);

            if (
                minutes < 0 ||
                minutes > 59
            )
            {
                return false;
            }

            /*
             * 8 horas es el máximo.
             * 8.00 sí.
             * 8.01 no.
             */
            if (
                wholeHours == 8m &&
                minutes > 0
            )
            {
                return false;
            }

            return true;
        }


        private static decimal? ResolveAttendeeTotalHours(
    CreateTrainingReportAttendeeDto attendee
)
        {
            /*
             * Compatibilidad temporal:
             * el frontend anterior todavía no manda
             * HoursMonday...HoursSunday a nivel asistente.
             */
            if (!HasAttendeeDailyHours(attendee))
            {
                return null;
            }

            /*
             * Nuevo contrato:
             * el total siempre se calcula en backend.
             */
            return CalculateAttendeeDailyTotal(attendee);
        }

        private static string? ValidateAttendeeDayHours(
    int employeeId,
    string dayName,
    bool daySelected,
    decimal? hours
)
        {
            if (!daySelected && !hours.HasValue)
            {
                return null;
            }

            if (!daySelected && hours.HasValue)
            {
                return
                    $"El empleado con ID {employeeId} tiene horas para {dayName}, " +
                    "pero ese día no está seleccionado.";
            }

            

            if (
    hours.HasValue &&
    !IsValidHourMinute(
        hours.Value
    )
)
            {
                return
                    $"Las horas de {dayName} para el empleado con ID {employeeId} " +
                    "deben usar el formato horas.minutos, por ejemplo 2.30, " +
                    "con un máximo de 8.00 horas.";
            }

            return null;
        }

        private static string? ValidateAttendeeHours(
    CreateTrainingReportAttendeeDto attendee
)
        {
            /*
             * Compatibilidad temporal con el frontend actual.
             * Si todavía no manda horas a nivel asistente,
             * no aplicamos el nuevo contrato.
             */

            var error = ValidateAttendeeDayHours(
                attendee.EmployeeId,
                "lunes",
                attendee.DayMonday,
                attendee.HoursMonday
            );

            if (error is not null)
            {
                return error;
            }

            error = ValidateAttendeeDayHours(
                attendee.EmployeeId,
                "martes",
                attendee.DayTuesday,
                attendee.HoursTuesday
            );

            if (error is not null)
            {
                return error;
            }

            error = ValidateAttendeeDayHours(
                attendee.EmployeeId,
                "miércoles",
                attendee.DayWednesday,
                attendee.HoursWednesday
            );

            if (error is not null)
            {
                return error;
            }

            error = ValidateAttendeeDayHours(
                attendee.EmployeeId,
                "jueves",
                attendee.DayThursday,
                attendee.HoursThursday
            );

            if (error is not null)
            {
                return error;
            }

            error = ValidateAttendeeDayHours(
                attendee.EmployeeId,
                "viernes",
                attendee.DayFriday,
                attendee.HoursFriday
            );

            if (error is not null)
            {
                return error;
            }

            error = ValidateAttendeeDayHours(
                attendee.EmployeeId,
                "sábado",
                attendee.DaySaturday,
                attendee.HoursSaturday
            );

            if (error is not null)
            {
                return error;
            }

            return ValidateAttendeeDayHours(
                attendee.EmployeeId,
                "domingo",
                attendee.DaySunday,
                attendee.HoursSunday
            );
        }

    }
}