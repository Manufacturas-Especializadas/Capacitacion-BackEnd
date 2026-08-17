using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.TrainingReports.Commands
{
    public class UpdateTrainingReportCommandHandler(
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorage
    ): IRequestHandler<
        UpdateTrainingReportCommand,
        UpdateTrainingReportResult
    >
    {
        public async Task<UpdateTrainingReportResult> Handle(
            UpdateTrainingReportCommand request,
            CancellationToken cancellationToken
        )
        {

            // Toda la lógica de actualización va aquí.
            var requestError = ValidateRequest(request);

            if (requestError is not null)
            {
                return Invalid(requestError);
            }

            var report = await unitOfWork
                .TrainingReports
                .GetTrackedDetailsByIdAsync(
                    request.Id,
                    cancellationToken
                );

            if (report is null)
            {
                return new UpdateTrainingReportResult(
                    UpdateTrainingReportStatus.NotFound,
                    $"No se encontró el reporte con ID {request.Id}."
                );
            }

            var ownershipError = ValidateChildOwnership(
                request,
                report
            );

            if (ownershipError is not null)
            {
                return Invalid(ownershipError);
            }

            /*
             * Verificamos todos los temas antes de modificar
             * el reporte o subir archivos.
             */
            var topicsResult = await LoadTopicsAsync(
                request.Attendees,
                cancellationToken
            );

            if (topicsResult.Error is not null)
            {
                return Invalid(topicsResult.Error);
            }

            /*
             * Archivos nuevos que debemos limpiar si falla SQL.
             */
            var newBlobUrls = new List<string>();

            /*
             * Archivos anteriores que se eliminarán solamente
             * después de guardar correctamente en SQL.
             */
            var oldBlobUrls = new HashSet<string>();

            try
            {
                UpdateMainData(report, request);

                report.InstructorSignatureUrl =
                    await ReplaceSignatureAsync(
                        report.InstructorSignatureUrl,
                        request.InstructorSignature,
                        request.RemoveInstructorSignature,
                        $"report_{report.Id}_instructor.png",
                        newBlobUrls,
                        oldBlobUrls
                    );

                report.CoordinatorSignatureUrl =
                    await ReplaceSignatureAsync(
                        report.CoordinatorSignatureUrl,
                        request.CoordinatorSignature,
                        request.RemoveCoordinatorSignature,
                        $"report_{report.Id}_coordinator.png",
                        newBlobUrls,
                        oldBlobUrls
                    );

                report.SecuritySignatureUrl =
                    await ReplaceSignatureAsync(
                        report.SecuritySignatureUrl,
                        request.SecuritySignature,
                        request.RemoveSecuritySignature,
                        $"report_{report.Id}_security.png",
                        newBlobUrls,
                        oldBlobUrls
                    );

                SynchronizeUnionTypes(
                    report,
                    request.UnionTypes
                );

                await SynchronizeAttendeesAsync(
                    report,
                    request.Attendees,
                    topicsResult.Topics,
                    newBlobUrls,
                    oldBlobUrls
                );

                await unitOfWork.SaveChangesAsync(
                    cancellationToken
                );

                /*
                 * Ya que SQL se guardó correctamente,
                 * eliminamos los archivos anteriores.
                 */
                foreach (var oldUrl in oldBlobUrls)
                {
                    await blobStorage
                        .DeleteFileTrainingReportAsync(oldUrl);
                }

                return new UpdateTrainingReportResult(
                    UpdateTrainingReportStatus.Updated,
                    "Reporte de entrenamiento actualizado correctamente."
                );
            }
            catch
            {
                /*
                 * Si SQL o alguna operación posterior falla,
                 * quitamos los archivos nuevos para evitar
                 * blobs huérfanos.
                 */
                foreach (var newUrl in newBlobUrls)
                {
                    await blobStorage
                        .DeleteFileTrainingReportAsync(newUrl);
                }

                throw;
            }

            //throw new NotImplementedException();
        }

        // Métodos privados de validación y sincronización.
        private static bool HasDailyHours(
    UpdateTrainingReportAttendeeTopicDto topic
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
            UpdateTrainingReportAttendeeTopicDto topic
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


        private static decimal CalculateStoredDailyTotal(
            TrainingReportAttendeeTopic topic
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
            if (!daySelected && !hours.HasValue)
            {
                return null;
            }


            if (!daySelected && hours.HasValue)
            {
                return
                    $"El tema con ID {topicId} tiene horas para {dayName}, " +
                    "pero ese día no está seleccionado.";
            }


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
            UpdateTrainingReportAttendeeTopicDto topic
        )
        {
            /*
             * Petición anterior o registro histórico.
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


        private static string? ValidateRequest(
            UpdateTrainingReportCommand request
        )
        {
            if (request.Id <= 0)
            {
                return
                    "El identificador del reporte no es válido.";
            }

            if (string.IsNullOrWhiteSpace(
                request.TrainingType
            ))
            {
                return
                    "El tipo de entrenamiento es obligatorio.";
            }

            if (string.IsNullOrWhiteSpace(
                request.LeaderName
            ))
            {
                return
                    "El nombre del líder es obligatorio.";
            }

            if (string.IsNullOrWhiteSpace(
                request.LeaderPayroll
            ))
            {
                return
                    "La nómina del líder es obligatoria.";
            }

            if (request.Attendees is null)
            {
                return
                    "La lista de asistentes es obligatoria.";
            }

            if (request.UnionTypes is null)
            {
                return
                    "La lista de uniones no puede ser nula.";
            }

            foreach (var union in request.UnionTypes)
            {
                if (union.Id.HasValue && union.Id <= 0)
                {
                    return
                        "La petición contiene un ID de unión no válido.";
                }

                if (string.IsNullOrWhiteSpace(
                    union.UnionName
                ))
                {
                    return
                        "El nombre de la unión es obligatorio.";
                }
            }

            foreach (var attendee in request.Attendees)
            {
                if (
                    attendee.Id.HasValue &&
                    attendee.Id <= 0
                )
                {
                    return
                        "La petición contiene un ID de asistente no válido.";
                }

                if (
                    attendee.Topics is { Count: > 0 } &&
                    attendee.Topics
                        .GroupBy(topic => topic.TopicId)
                        .Any(group => group.Count() > 1)
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


                foreach (
                    var topicAssignment in topicAssignments
                )
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
                    attendee.RemoveTraineeSignature &&
                    HasFile(attendee.TraineeSignature)
                )
                {
                    return
                        "No se puede eliminar y reemplazar al mismo tiempo la firma del asistente.";
                }

                if (
                    attendee.RemoveSupervisorSignature &&
                    HasFile(attendee.SupervisorSignature)
                )
                {
                    return
                        "No se puede eliminar y reemplazar al mismo tiempo la firma del supervisor.";
                }
            }

            if (
                request.RemoveInstructorSignature &&
                HasFile(request.InstructorSignature)
            )
            {
                return
                    "No se puede eliminar y reemplazar al mismo tiempo la firma del instructor.";
            }

            if (
                request.RemoveCoordinatorSignature &&
                HasFile(request.CoordinatorSignature)
            )
            {
                return
                    "No se puede eliminar y reemplazar al mismo tiempo la firma del coordinador.";
            }

            if (
                request.RemoveSecuritySignature &&
                HasFile(request.SecuritySignature)
            )
            {
                return
                    "No se puede eliminar y reemplazar al mismo tiempo la firma de seguridad.";
            }

            var duplicatedAttendeeIds = request.Attendees
                .Where(item => item.Id.HasValue)
                .GroupBy(item => item.Id!.Value)
                .Any(group => group.Count() > 1);

            if (duplicatedAttendeeIds)
            {
                return
                    "La petición contiene asistentes duplicados.";
            }

            var duplicatedUnionIds = request.UnionTypes
                .Where(item => item.Id.HasValue)
                .GroupBy(item => item.Id!.Value)
                .Any(group => group.Count() > 1);

            if (duplicatedUnionIds)
            {
                return
                    "La petición contiene tipos de unión duplicados.";
            }

            return null;
        }

        private static List<UpdateTrainingReportAttendeeTopicDto>
    ResolveTopicAssignments(
        UpdateTrainingReportAttendeeDto attendee)
        {
            /*
             * Nuevo contrato:
             * cada tema ya trae sus propios días y horas.
             */
            if (attendee.Topics is { Count: > 0 })
            {
                return attendee.Topics
                    .GroupBy(topic => topic.TopicId)
                    .Select(group => group.First())
                    .ToList();
            }

            /*
             * Compatibilidad temporal con el frontend anterior:
             * TopicIds + días almacenados a nivel asistente.
             */
            return (attendee.TopicIds ?? new List<int>())
                .Distinct()
                .Select(topicId =>
                    new UpdateTrainingReportAttendeeTopicDto
                    {
                        TopicId = topicId,

                        DayMonday = attendee.DayMonday,
                        DayTuesday = attendee.DayTuesday,
                        DayWednesday = attendee.DayWednesday,
                        DayThursday = attendee.DayThursday,
                        DayFriday = attendee.DayFriday,
                        DaySaturday = attendee.DaySaturday,
                        DaySunday = attendee.DaySunday,

                        /*
                         * El contrato anterior no capturaba horas.
                         */
                        TotalHours = null
                    })
                .ToList();
        }

        private static bool HasFile(IFormFile? file)
        {
            return file is not null && file.Length > 0;
        }

        private static string? ValidateChildOwnership(
            UpdateTrainingReportCommand request,
            TrainingReport report
        )
        {
            var attendeeIds = report.Attendees
                .Select(attendee => attendee.Id)
                .ToHashSet();

            var invalidAttendeeId = request.Attendees
                .Where(item => item.Id.HasValue)
                .Select(item => item.Id!.Value)
                .FirstOrDefault(
                    id => !attendeeIds.Contains(id)
                );

            if (invalidAttendeeId > 0)
            {
                return
                    $"El asistente con ID {invalidAttendeeId} no pertenece al reporte.";
            }

            var unionIds = report.WeldingUnionTypes
                .Select(union => union.Id)
                .ToHashSet();

            var invalidUnionId = request.UnionTypes
                .Where(item => item.Id.HasValue)
                .Select(item => item.Id!.Value)
                .FirstOrDefault(
                    id => !unionIds.Contains(id)
                );

            if (invalidUnionId > 0)
            {
                return
                    $"La unión con ID {invalidUnionId} no pertenece al reporte.";
            }

            return null;
        }

        private async Task<TopicLoadResult> LoadTopicsAsync(
            IEnumerable<UpdateTrainingReportAttendeeDto>
                attendees,
            CancellationToken cancellationToken
        )
        {
            var topicIds = attendees
                .SelectMany(
                    attendee =>
                        ResolveTopicAssignments(attendee)
                .Select(topic => topic.TopicId)
                )
                .Distinct()
                .ToList();

            var topics = new Dictionary<int, TrainingTopic>();

            foreach (var topicId in topicIds)
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

                var topic = await unitOfWork
                    .TrainingTopics
                    .GetByIdAsync(topicId);

                if (topic is null)
                {
                    return new TopicLoadResult(
                        topics,
                        $"No se encontró el tema con ID {topicId}."
                    );
                }

                topics[topic.Id] = topic;
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

        private static void UpdateMainData(
            TrainingReport report,
            UpdateTrainingReportCommand request
        )
        {
            report.TrainingType =
                request.TrainingType.Trim();

            report.LeaderName =
                request.LeaderName.Trim();

            report.LeaderPayroll =
                request.LeaderPayroll.Trim();

            report.WeekNumber =
                request.WeekNumber;

            report.Observations =
                Normalize(request.Observations);

        }

        private async Task<string?> ReplaceSignatureAsync(
            string? currentUrl,
            IFormFile? newFile,
            bool removeCurrent,
            string fileName,
            ICollection<string> newBlobUrls,
            ISet<string> oldBlobUrls
        )
        {
            if (removeCurrent)
            {
                AddUrl(oldBlobUrls, currentUrl);
                return null;
            }

            if (!HasFile(newFile))
            {
                /*
                 * No llegó una firma nueva:
                 * conservamos la anterior.
                 */
                return currentUrl;
            }

            var newUrl = await blobStorage
                .UploadFileTrainingReportAsync(
                    newFile!,
                    fileName
                );

            if (string.IsNullOrWhiteSpace(newUrl))
            {
                throw new InvalidOperationException(
                    "No se pudo almacenar la nueva firma."
                );
            }

            newBlobUrls.Add(newUrl);
            AddUrl(oldBlobUrls, currentUrl);

            return newUrl;
        }

        private void SynchronizeUnionTypes(
            TrainingReport report,
            IReadOnlyCollection<UpdateWeldingUnionTypeDto>
                requestedUnionTypes
        )
        {
            var requestedIds = requestedUnionTypes
                .Where(union => union.Id.HasValue)
                .Select(union => union.Id!.Value)
                .ToHashSet();

            var unionsToDelete = report
                .WeldingUnionTypes
                .Where(
                    existing =>
                        !requestedIds.Contains(existing.Id)
                )
                .ToList();

            if (unionsToDelete.Count > 0)
            {
                unitOfWork
                    .TrainingReports
                    .DeleteUnionTypes(unionsToDelete);
            }

            var existingById = report
                .WeldingUnionTypes
                .ToDictionary(union => union.Id);

            foreach (var requested in requestedUnionTypes)
            {
                if (requested.Id.HasValue)
                {
                    var existing =
                        existingById[requested.Id.Value];

                    existing.ListNumber =
                        requested.ListNumber;

                    existing.UnionName =
                        requested.UnionName.Trim();

                    continue;
                }

                report.WeldingUnionTypes.Add(
                    new WeldingReportUnionType
                    {
                        ListNumber =
                            requested.ListNumber,

                        UnionName =
                            requested.UnionName.Trim()
                    }
                );
            }
        }

        private static void UpdateTopicAssignmentData(
    TrainingReportAttendeeTopic assignment,
    UpdateTrainingReportAttendeeTopicDto requested
)
        {
            // =========================================================
            // DÍAS
            // =========================================================

            assignment.DayMonday =
                requested.DayMonday;

            assignment.DayTuesday =
                requested.DayTuesday;

            assignment.DayWednesday =
                requested.DayWednesday;

            assignment.DayThursday =
                requested.DayThursday;

            assignment.DayFriday =
                requested.DayFriday;

            assignment.DaySaturday =
                requested.DaySaturday;

            assignment.DaySunday =
                requested.DaySunday;


            // =========================================================
            // CONTRATO NUEVO
            //
            // Llegaron horas individuales.
            // =========================================================

            if (HasDailyHours(requested))
            {
                assignment.HoursMonday =
                    requested.HoursMonday;

                assignment.HoursTuesday =
                    requested.HoursTuesday;

                assignment.HoursWednesday =
                    requested.HoursWednesday;

                assignment.HoursThursday =
                    requested.HoursThursday;

                assignment.HoursFriday =
                    requested.HoursFriday;

                assignment.HoursSaturday =
                    requested.HoursSaturday;

                assignment.HoursSunday =
                    requested.HoursSunday;


                /*
                 * Ignoramos TotalHours enviado por el cliente
                 * y lo calculamos nosotros.
                 */
                assignment.TotalHours =
                    CalculateDailyTotal(
                        requested
                    );

                return;
            }


            // =========================================================
            // CONTRATO LEGACY / HISTÓRICO
            //
            // No llegaron HoursX.
            //
            // No queremos borrar automáticamente información diaria
            // que ya pudiera existir en la base de datos.
            // =========================================================


            /*
             * Si el usuario desmarcó un día, sí debemos eliminar
             * sus horas almacenadas.
             *
             * Esto además mantiene consistencia con los CHECK
             * constraints de SQL.
             */
            if (!requested.DayMonday)
            {
                assignment.HoursMonday = null;
            }

            if (!requested.DayTuesday)
            {
                assignment.HoursTuesday = null;
            }

            if (!requested.DayWednesday)
            {
                assignment.HoursWednesday = null;
            }

            if (!requested.DayThursday)
            {
                assignment.HoursThursday = null;
            }

            if (!requested.DayFriday)
            {
                assignment.HoursFriday = null;
            }

            if (!requested.DaySaturday)
            {
                assignment.HoursSaturday = null;
            }

            if (!requested.DaySunday)
            {
                assignment.HoursSunday = null;
            }


            /*
             * Revisamos si todavía conservamos
             * alguna hora diaria conocida.
             */
            var hasStoredDailyHours =
                assignment.HoursMonday.HasValue ||
                assignment.HoursTuesday.HasValue ||
                assignment.HoursWednesday.HasValue ||
                assignment.HoursThursday.HasValue ||
                assignment.HoursFriday.HasValue ||
                assignment.HoursSaturday.HasValue ||
                assignment.HoursSunday.HasValue;


            /*
             * ¿Conocemos las horas de TODOS los días
             * actualmente seleccionados?
             */
            var allSelectedDaysHaveHours =
                (
                    !requested.DayMonday ||
                    assignment.HoursMonday.HasValue
                )
                &&
                (
                    !requested.DayTuesday ||
                    assignment.HoursTuesday.HasValue
                )
                &&
                (
                    !requested.DayWednesday ||
                    assignment.HoursWednesday.HasValue
                )
                &&
                (
                    !requested.DayThursday ||
                    assignment.HoursThursday.HasValue
                )
                &&
                (
                    !requested.DayFriday ||
                    assignment.HoursFriday.HasValue
                )
                &&
                (
                    !requested.DaySaturday ||
                    assignment.HoursSaturday.HasValue
                )
                &&
                (
                    !requested.DaySunday ||
                    assignment.HoursSunday.HasValue
                );


            /*
             * Si conocemos toda la distribución diaria,
             * mantenemos el total sincronizado.
             */
            if (
                hasStoredDailyHours &&
                allSelectedDaysHaveHours
            )
            {
                assignment.TotalHours =
                    CalculateStoredDailyTotal(
                        assignment
                    );

                return;
            }


            /*
             * Si todavía estamos ante un registro histórico
             * o una petición del frontend anterior,
             * conservamos el TotalHours recibido.
             */
            assignment.TotalHours =
                requested.TotalHours;
        }

        private async Task SynchronizeAttendeesAsync(
            TrainingReport report,
            IReadOnlyCollection<
                UpdateTrainingReportAttendeeDto
            > requestedAttendees,
            IReadOnlyDictionary<int, TrainingTopic>
                topicsById,
            ICollection<string> newBlobUrls,
            ISet<string> oldBlobUrls
        )
        {
            var requestedIds = requestedAttendees
                .Where(attendee => attendee.Id.HasValue)
                .Select(attendee => attendee.Id!.Value)
                .ToHashSet();

            var attendeesToDelete = report.Attendees
                .Where(
                    existing =>
                        !requestedIds.Contains(existing.Id)
                )
                .ToList();

            foreach (var attendee in attendeesToDelete)
            {
                AddUrl(
                    oldBlobUrls,
                    attendee.TraineeSignatureUrl
                );

                AddUrl(
                    oldBlobUrls,
                    attendee.SupervisorSignatureUrl
                );

                /*
                 * Elimina las relaciones de la tabla
                 * intermedia asistente-tema.
                 */
                attendee.Topics.Clear();
            }

            if (attendeesToDelete.Count > 0)
            {
                unitOfWork
                    .TrainingReports
                    .DeleteAttendees(attendeesToDelete);
            }

            var existingById = report.Attendees
                .ToDictionary(attendee => attendee.Id);

            foreach (var requested in requestedAttendees)
            {
                TrainingReportAttendee attendee;

                if (requested.Id.HasValue)
                {
                    attendee =
                        existingById[requested.Id.Value];
                }
                else
                {
                    attendee =
                        new TrainingReportAttendee();

                    report.Attendees.Add(attendee);
                }

                var topicAssignments = ResolveTopicAssignments(requested);

                UpdateAttendeeData(
                    attendee,
                    requested,
                    topicAssignments
                );

                attendee.TraineeSignatureUrl =
                    await ReplaceSignatureAsync(
                        attendee.TraineeSignatureUrl,
                        requested.TraineeSignature,
                        requested.RemoveTraineeSignature,
                        $"report_{report.Id}_" +
                        $"employee_{requested.EmployeeId}_" +
                        "trainee.png",
                        newBlobUrls,
                        oldBlobUrls
                    );

                attendee.SupervisorSignatureUrl =
                    await ReplaceSignatureAsync(
                        attendee.SupervisorSignatureUrl,
                        requested.SupervisorSignature,
                        requested.RemoveSupervisorSignature,
                        $"report_{report.Id}_" +
                        $"employee_{requested.EmployeeId}_" +
                        "supervisor.png",
                        newBlobUrls,
                        oldBlobUrls
                    );

                /*
 * Sincronizamos las asignaciones de temas
 * sin eliminar los temas del catálogo.
 */
                var requestedTopicIds =
                    topicAssignments
                        .Select(topic => topic.TopicId)
                        .ToHashSet();

                /*
                 * Relaciones que existían en BD pero que el
                 * usuario eliminó durante la edición.
                 */
                var topicAssignmentsToDelete =
                    attendee.Topics
                        .Where(
                            existing =>
                                !requestedTopicIds.Contains(
                                    existing.TopicId
                                )
                        )
                        .ToList();

                foreach (
                    var topicAssignmentToDelete
                        in topicAssignmentsToDelete
                )
                {
                    attendee.Topics.Remove(
                        topicAssignmentToDelete
                    );
                }

                /*
                 * Después de quitar las relaciones que ya no
                 * fueron solicitadas, indexamos las restantes.
                 */
                var existingTopicsById =
                    attendee.Topics
                        .ToDictionary(
                            assignment => assignment.TopicId
                        );

                foreach (var requestedTopic in topicAssignments)
                {
                    /*
                     * Tema que ya pertenecía al asistente:
                     * solamente actualizamos sus datos.
                     */
                    if (
                        existingTopicsById.TryGetValue(
                            requestedTopic.TopicId,
                            out var existingTopic
                        )
                    )
                    {
                        UpdateTopicAssignmentData(
                            existingTopic,
                            requestedTopic
                        );

                        continue;
                    }

                    /*
                     * Tema nuevo para este asistente.
                     */
                    var newTopicAssignment =
                        new TrainingReportAttendeeTopic
                        {
                            TopicId = requestedTopic.TopicId,

                            Topic =
                                topicsById[
                                    requestedTopic.TopicId
                                ],

                            Attendee = attendee
                        };

                    UpdateTopicAssignmentData(
                        newTopicAssignment,
                        requestedTopic
                    );

                    attendee.Topics.Add(
                        newTopicAssignment
                    );
                }
            }
        }

        private static void UpdateAttendeeData(
            TrainingReportAttendee attendee,
            UpdateTrainingReportAttendeeDto requested,
            IReadOnlyCollection<UpdateTrainingReportAttendeeTopicDto> topicAssignments
        )
        {
            attendee.EmployeeId =
                requested.EmployeeId;

            attendee.LineId =
                requested.LineId;

            /*
 * Campos legacy.
 *
 * Conservamos la unión de los días de todos
 * los temas mientras estas columnas sigan
 * existiendo en TrainingReportAttendees.
 */
            attendee.DayMonday =
                topicAssignments.Any(
                    topic => topic.DayMonday
                );

            attendee.DayTuesday =
                topicAssignments.Any(
                    topic => topic.DayTuesday
                );

            attendee.DayWednesday =
                topicAssignments.Any(
                    topic => topic.DayWednesday
                );

            attendee.DayThursday =
                topicAssignments.Any(
                    topic => topic.DayThursday
                );

            attendee.DayFriday =
                topicAssignments.Any(
                    topic => topic.DayFriday
                );

            attendee.DaySaturday =
                topicAssignments.Any(
                    topic => topic.DaySaturday
                );

            attendee.DaySunday =
                topicAssignments.Any(
                    topic => topic.DaySunday
                );

            attendee.CustomerClient =
                Normalize(requested.CustomerClient);

            attendee.UnionClassification =
                Normalize(
                    requested.UnionClassification
                );

            attendee.WeldingPercentage =
                Normalize(
                    requested.WeldingPercentage
                );

            attendee.Diameter =
                Normalize(requested.Diameter);

            attendee.Shift =
                Normalize(requested.Shift);

            attendee.Machinery =
                Normalize(requested.Machinery);

            attendee.Ast =
                Normalize(requested.Ast);
        }

        private static string? Normalize(
            string? value
        )
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }

        private static void AddUrl(
            ISet<string> urls,
            string? url
        )
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                urls.Add(url);
            }
        }

        private static UpdateTrainingReportResult Invalid(
            string message
        )
        {
            return new UpdateTrainingReportResult(
                UpdateTrainingReportStatus.InvalidRequest,
                message
            );
        }



    }
}
