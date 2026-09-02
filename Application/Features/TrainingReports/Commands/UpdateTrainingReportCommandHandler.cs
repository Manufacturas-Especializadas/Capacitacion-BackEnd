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

        private static bool HasAttendeeDailyHours(
    UpdateTrainingReportAttendeeDto attendee
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
    UpdateTrainingReportAttendeeDto attendee
)
        {
            var totalMinutes =
                ConvertAttendeeHourMinuteToMinutes(
                    attendee.HoursMonday
                ) +
                ConvertAttendeeHourMinuteToMinutes(
                    attendee.HoursTuesday
                ) +
                ConvertAttendeeHourMinuteToMinutes(
                    attendee.HoursWednesday
                ) +
                ConvertAttendeeHourMinuteToMinutes(
                    attendee.HoursThursday
                ) +
                ConvertAttendeeHourMinuteToMinutes(
                    attendee.HoursFriday
                ) +
                ConvertAttendeeHourMinuteToMinutes(
                    attendee.HoursSaturday
                ) +
                ConvertAttendeeHourMinuteToMinutes(
                    attendee.HoursSunday
                );

            return ConvertAttendeeMinutesToHourMinute(
                totalMinutes
            );
        }
        private static int ConvertAttendeeHourMinuteToMinutes(
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


        private static decimal ConvertAttendeeMinutesToHourMinute(
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

        private static bool IsValidAttendeeHourMinute(
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

            if (
                wholeHours == 8m &&
                minutes > 0
            )
            {
                return false;
            }

            return true;
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

            if (daySelected && !hours.HasValue)
            {
                return
                    $"Debes indicar las horas de {dayName} " +
                    $"para el empleado con ID {employeeId}.";
            }

            if (
    hours.HasValue &&
    !IsValidAttendeeHourMinute(
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
    UpdateTrainingReportAttendeeDto attendee
)
        {
            /*
             * Compatibilidad temporal con el frontend anterior.
             * Si todavía no manda HoursX a nivel asistente,
             * permitimos la petición.
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
    .Select(
        topicId =>
            new UpdateTrainingReportAttendeeTopicDto
            {
                TopicId = topicId
            }
    )
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
                    requested
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
                     * Si la relación asistente-tema ya existe,
                     * no hay ningún dato adicional que actualizar.
                     */
                    if (
                        existingTopicsById.ContainsKey(
                            requestedTopic.TopicId
                        )
                    )
                    {
                        continue;
                    }

                    /*
                     * Si es un tema nuevo, únicamente
                     * creamos la relación.
                     */
                    attendee.Topics.Add(
                        new TrainingReportAttendeeTopic
                        {
                            TopicId =
                                requestedTopic.TopicId,

                            Topic =
                                topicsById[
                                    requestedTopic.TopicId
                                ],

                            Attendee =
                                attendee
                        }
                    );
                }
            }
        }

        private static void UpdateAttendeeData(
    TrainingReportAttendee attendee,
    UpdateTrainingReportAttendeeDto requested
)
        {
            attendee.EmployeeId =
                requested.EmployeeId;

            attendee.LineId =
                requested.LineId;


            /*
             * Los días ahora pertenecen directamente
             * al asistente, no a sus temas.
             */
            attendee.DayMonday =
                requested.DayMonday;

            attendee.DayTuesday =
                requested.DayTuesday;

            attendee.DayWednesday =
                requested.DayWednesday;

            attendee.DayThursday =
                requested.DayThursday;

            attendee.DayFriday =
                requested.DayFriday;

            attendee.DaySaturday =
                requested.DaySaturday;

            attendee.DaySunday =
                requested.DaySunday;


            attendee.HoursMonday =
    requested.HoursMonday;

            attendee.HoursTuesday =
                requested.HoursTuesday;

            attendee.HoursWednesday =
                requested.HoursWednesday;

            attendee.HoursThursday =
                requested.HoursThursday;

            attendee.HoursFriday =
                requested.HoursFriday;

            attendee.HoursSaturday =
                requested.HoursSaturday;

            attendee.HoursSunday =
                requested.HoursSunday;

            attendee.TotalHours =
                HasAttendeeDailyHours(requested)
                    ? CalculateAttendeeDailyTotal(
                        requested
                    )
                    : null;


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
