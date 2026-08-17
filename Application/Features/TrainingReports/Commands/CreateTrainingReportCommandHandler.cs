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

        public decimal? TotalHours { get; set; }
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
                                topicAssignments.Any(
                                    topic =>
                                        topic.DayMonday
                                ),

                            DayTuesday =
                                topicAssignments.Any(
                                    topic =>
                                        topic.DayTuesday
                                ),

                            DayWednesday =
                                topicAssignments.Any(
                                    topic =>
                                        topic.DayWednesday
                                ),

                            DayThursday =
                                topicAssignments.Any(
                                    topic =>
                                        topic.DayThursday
                                ),

                            DayFriday =
                                topicAssignments.Any(
                                    topic =>
                                        topic.DayFriday
                                ),

                            DaySaturday =
                                topicAssignments.Any(
                                    topic =>
                                        topic.DaySaturday
                                ),

                            DaySunday =
                                topicAssignments.Any(
                                    topic =>
                                        topic.DaySunday
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
        TopicId =
            topic.Id,

        DayMonday =
            topicAssignment.DayMonday,

        DayTuesday =
            topicAssignment.DayTuesday,

        DayWednesday =
            topicAssignment.DayWednesday,

        DayThursday =
            topicAssignment.DayThursday,

        DayFriday =
            topicAssignment.DayFriday,

        DaySaturday =
            topicAssignment.DaySaturday,

        DaySunday =
            topicAssignment.DaySunday,


        HoursMonday =
            topicAssignment.HoursMonday,

        HoursTuesday =
            topicAssignment.HoursTuesday,

        HoursWednesday =
            topicAssignment.HoursWednesday,

        HoursThursday =
            topicAssignment.HoursThursday,

        HoursFriday =
            topicAssignment.HoursFriday,

        HoursSaturday =
            topicAssignment.HoursSaturday,

        HoursSunday =
            topicAssignment.HoursSunday,


        /*
         * Nuevo contrato:
         * calculado automáticamente.
         *
         * Contrato anterior:
         * conserva TotalHours enviado.
         */
        TotalHours =
            ResolveTotalHours(
                topicAssignment
            ),


        Topic =
            topic,

        Attendee =
            attendee
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


        private static bool HasDailyHours(
    CreateTrainingReportAttendeeTopicDto topic
)
        {
            return
                topic.HoursMonday.HasValue ||
                topic.HoursTuesday.HasValue ||
                topic.HoursWednesday.HasValue ||
                topic.HoursThursday.HasValue ||
                topic.HoursFriday.HasValue ||
                topic.HoursSaturday.HasValue ||
                topic.HoursSunday.HasValue;
        }


        private static decimal CalculateDailyTotal(
            CreateTrainingReportAttendeeTopicDto topic
        )
        {
            return
                (topic.HoursMonday ?? 0m) +
                (topic.HoursTuesday ?? 0m) +
                (topic.HoursWednesday ?? 0m) +
                (topic.HoursThursday ?? 0m) +
                (topic.HoursFriday ?? 0m) +
                (topic.HoursSaturday ?? 0m) +
                (topic.HoursSunday ?? 0m);
        }


        private static string? ValidateDayHours(
            int topicId,
            string dayName,
            bool daySelected,
            decimal? hours
        )
        {
            /*
             * Si no hay horas y tampoco está seleccionado,
             * es completamente válido.
             */
            if (!daySelected && !hours.HasValue)
            {
                return null;
            }


            /*
             * No permitimos horas en un día que
             * no fue seleccionado.
             */
            if (!daySelected && hours.HasValue)
            {
                return
                    $"El tema con ID {topicId} tiene horas para {dayName}, " +
                    "pero ese día no está seleccionado.";
            }


            /*
             * Este método solamente se utiliza para el
             * contrato nuevo.
             *
             * Si el día está seleccionado, debe conocer
             * sus horas.
             */
            if (daySelected && !hours.HasValue)
            {
                return
                    $"Debes indicar las horas de {dayName} " +
                    $"para el tema con ID {topicId}.";
            }


            if (
                hours.HasValue &&
                (
                    hours.Value < 0m ||
                    hours.Value > 8m
                )
            )
            {
                return
                    $"Las horas de {dayName} para el tema con ID {topicId} " +
                    "deben estar entre 0 y 8.";
            }


            return null;
        }


        private static string? ValidateTopicHours(
            CreateTrainingReportAttendeeTopicDto topic
        )
        {
            /*
             * CONTRATO LEGACY / HISTÓRICO
             *
             * Si no llegó ninguna hora diaria,
             * todavía aceptamos TotalHours.
             */
            if (!HasDailyHours(topic))
            {
                if (
                    topic.TotalHours.HasValue &&
                    (
                        topic.TotalHours.Value < 0m ||
                        topic.TotalHours.Value > 56m
                    )
                )
                {
                    return
                        $"Las horas totales del tema con ID {topic.TopicId} " +
                        "deben estar entre 0 y 56.";
                }

                return null;
            }


            /*
             * CONTRATO NUEVO
             *
             * En cuanto llega al menos una HoursX,
             * todos los días seleccionados deben
             * especificar sus horas.
             */
            var error = ValidateDayHours(
                topic.TopicId,
                "lunes",
                topic.DayMonday,
                topic.HoursMonday
            );

            if (error is not null)
            {
                return error;
            }


            error = ValidateDayHours(
                topic.TopicId,
                "martes",
                topic.DayTuesday,
                topic.HoursTuesday
            );

            if (error is not null)
            {
                return error;
            }


            error = ValidateDayHours(
                topic.TopicId,
                "miércoles",
                topic.DayWednesday,
                topic.HoursWednesday
            );

            if (error is not null)
            {
                return error;
            }


            error = ValidateDayHours(
                topic.TopicId,
                "jueves",
                topic.DayThursday,
                topic.HoursThursday
            );

            if (error is not null)
            {
                return error;
            }


            error = ValidateDayHours(
                topic.TopicId,
                "viernes",
                topic.DayFriday,
                topic.HoursFriday
            );

            if (error is not null)
            {
                return error;
            }


            error = ValidateDayHours(
                topic.TopicId,
                "sábado",
                topic.DaySaturday,
                topic.HoursSaturday
            );

            if (error is not null)
            {
                return error;
            }


            return ValidateDayHours(
                topic.TopicId,
                "domingo",
                topic.DaySunday,
                topic.HoursSunday
            );
        }


        private static decimal? ResolveTotalHours(
            CreateTrainingReportAttendeeTopicDto topic
        )
        {
            /*
             * Contrato anterior:
             * confiamos temporalmente en TotalHours.
             */
            if (!HasDailyHours(topic))
            {
                return topic.TotalHours;
            }


            /*
             * Contrato nuevo:
             * TotalHours recibido deja de ser la fuente
             * de verdad.
             */
            return CalculateDailyTotal(topic);
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

                foreach (var topicAssignment in topicAssignments)
                {
                    var hoursError =
                        ValidateTopicHours(
                            topicAssignment
                        );

                    if (hoursError is not null)
                    {
                        return hoursError;
                    }
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
                            TopicId =
                                topicId,

                            DayMonday =
                                attendee.DayMonday,

                            DayTuesday =
                                attendee.DayTuesday,

                            DayWednesday =
                                attendee.DayWednesday,

                            DayThursday =
                                attendee.DayThursday,

                            DayFriday =
                                attendee.DayFriday,

                            DaySaturday =
                                attendee.DaySaturday,

                            DaySunday =
                                attendee.DaySunday,

                            /*
                             * El frontend anterior nunca
                             * capturó horas.
                             */
                            TotalHours =
                                null
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
    }
}