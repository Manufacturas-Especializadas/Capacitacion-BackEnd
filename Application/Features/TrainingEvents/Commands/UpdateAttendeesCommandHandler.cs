using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingEvents.Commands
{
    public enum UpdateAttendeesStatus
    {
        Updated,
        InvalidRequest,
        NotFound
    }

    public sealed record UpdateAttendeesResult(
        UpdateAttendeesStatus Status,
        string Message
    );

    public record UpdateAttendeesCommand(
        int EventId,
        AssignAttendeesDto Data
    ) : IRequest<UpdateAttendeesResult>;

    public class UpdateAttendeesCommandHandler(
        IUnitOfWork unitOfWork
    ) : IRequestHandler<
        UpdateAttendeesCommand,
        UpdateAttendeesResult
    >
    {
        private readonly IUnitOfWork _unitOfWork =
            unitOfWork;

        public async Task<UpdateAttendeesResult> Handle(
            UpdateAttendeesCommand request,
            CancellationToken cancellationToken
        )
        {
            if (
                request.EventId <= 0 ||
                request.Data.EventId != request.EventId
            )
            {
                return Invalid(
                    "El identificador del evento no es válido."
                );
            }

            if (
                request.Data.Attendees is null ||
                request.Data.Attendees.Count == 0
            )
            {
                return Invalid(
                    "Debe existir al menos un participante."
                );
            }

            var trainingEvent =
                await _unitOfWork
                    .TrainingEvents
                    .GetEventWithDetailAsync(
                        request.EventId
                    );

            if (trainingEvent is null)
            {
                return new UpdateAttendeesResult(
                    UpdateAttendeesStatus.NotFound,
                    $"No se encontró el evento con ID {request.EventId}."
                );
            }

            var topics =
                trainingEvent.Topics
                    .OrderBy(topic => topic.TopicOrder)
                    .ToList();

            if (topics.Count == 0)
            {
                return Invalid(
                    "El evento no tiene temas configurados."
                );
            }

            /*
             * Validamos toda la petición antes de modificar
             * entidades en memoria.
             */
            foreach (
                var row in request.Data.Attendees
            )
            {
                if (
                    string.IsNullOrWhiteSpace(
                        row.EmployeeNumber
                    )
                )
                {
                    return Invalid(
                        "Todos los participantes deben tener número de nómina."
                    );
                }

                if (
                    string.IsNullOrWhiteSpace(
                        row.Name
                    )
                )
                {
                    return Invalid(
                        $"El participante {row.EmployeeNumber} debe tener nombre."
                    );
                }

                if (
                    string.IsNullOrWhiteSpace(
                        row.LineName
                    )
                )
                {
                    return Invalid(
                        $"El participante {row.EmployeeNumber} debe tener línea."
                    );
                }

                if (
                    row.Enrollments.Count !=
                    topics.Count
                )
                {
                    return Invalid(
                        $"La matriz de temas del participante {row.EmployeeNumber} " +
                        "no coincide con los temas del evento."
                    );
                }
            }

            var duplicatedEmployees =
                request.Data.Attendees
                    .GroupBy(
                        attendee =>
                            attendee
                                .EmployeeNumber
                                .Trim(),
                        StringComparer.OrdinalIgnoreCase
                    )
                    .Any(group =>
                        group.Count() > 1
                    );

            if (duplicatedEmployees)
            {
                return Invalid(
                    "No se puede asignar al mismo participante más de una vez."
                );
            }

            /*
             * La petición representa el estado FINAL
             * que debe tener la lista de participantes.
             */
            var requestedEmployeeNumbers =
                request.Data.Attendees
                    .Select(
                        attendee =>
                            attendee
                                .EmployeeNumber
                                .Trim()
                    )
                    .ToHashSet(
                        StringComparer.OrdinalIgnoreCase
                    );

            var attendeesToRemove =
                trainingEvent.Attendees
                    .Where(
                        attendee =>
                            attendee.Employee is not null &&
                            !requestedEmployeeNumbers.Contains(
                                attendee.Employee
                                    .EmployeeNumber
                                    .Trim()
                            )
                    )
                    .ToList();

            /*
             * Protección de evidencia histórica.
             *
             * No permitimos borrar un participante
             * que ya tenga asistencia, calificación
             * o firma capturada.
             */
            foreach (
                var attendee in attendeesToRemove
            )
            {
                if (
                    HasCapturedInformation(
                        attendee
                    )
                )
                {
                    return Invalid(
                        $"No se puede eliminar a " +
                        $"{attendee.Employee?.Name ?? "este participante"} " +
                        "porque ya tiene asistencia, calificación " +
                        "o firma registrada."
                    );
                }
            }

            /*
             * Agregamos participantes nuevos o
             * actualizamos sus inscripciones.
             */
            foreach (
                var row in request.Data.Attendees
            )
            {
                var employeeNumber =
                    row.EmployeeNumber.Trim();

                var employee =
                    await _unitOfWork
                        .Employees
                        .GetByEmployeeNumberAsync(
                            employeeNumber
                        );

                /*
                 * Conservamos el comportamiento del alta actual:
                 * si el empleado todavía no existe, se crea.
                 */
                if (employee is null)
                {
                    var line =
                        await _unitOfWork
                            .ProductionLines
                            .GetByNameAsync(
                                row.LineName.Trim()
                            );

                    if (line is null)
                    {
                        return Invalid(
                            $"La línea {row.LineName} no existe en el catálogo."
                        );
                    }

                    employee = new Domain.Entities.Employee
                        {
                            EmployeeNumber =
                                employeeNumber,

                            Name =
                                row.Name.Trim(),

                            LineId =
                                line.Id
                        };

                    await _unitOfWork
                        .Employees
                        .AddAsync(employee);
                }

                var existingAttendee =
                    trainingEvent.Attendees
                        .FirstOrDefault(
                            attendee =>
                                attendee.EmployeeId ==
                                employee.Id
                        );

                if (existingAttendee is null)
                {
                    /*
                     * Participante nuevo.
                     */
                    var newAttendee =
                        new EventAttendee
                        {
                            EventId =
                                trainingEvent.Id,

                            EmployeeId =
                                employee.Id,

                            Employee =
                                employee
                        };

                    for (
                        var index = 0;
                        index < topics.Count;
                        index++
                    )
                    {
                        newAttendee
                            .Evaluations
                            .Add(
                                new TopicEvaluation
                                {
                                    TopicId =
                                        topics[index].Id,

                                    IsEnrolled =
                                        row.Enrollments[
                                            index
                                        ],

                                    AttendanceStatus =
                                        "EMPTY",

                                    Grade =
                                        null
                                }
                            );
                    }

                    await _unitOfWork
                        .EventAttendees
                        .AddAsync(
                            newAttendee
                        );

                    continue;
                }

                /*
                 * Participante existente:
                 * NO destruimos sus evaluaciones.
                 *
                 * Solamente modificamos IsEnrolled.
                 */
                for (
                    var index = 0;
                    index < topics.Count;
                    index++
                )
                {
                    var topic =
                        topics[index];

                    var isEnrolled =
                        row.Enrollments[index];

                    var evaluation =
                        existingAttendee
                            .Evaluations
                            .FirstOrDefault(
                                item =>
                                    item.TopicId ==
                                    topic.Id
                            );

                    if (evaluation is null)
                    {
                        /*
                         * Si por alguna razón faltaba una
                         * celda para ese tema, la creamos.
                         */
                        evaluation =
                            new TopicEvaluation
                            {
                                AttendeeId =
                                    existingAttendee.Id,

                                TopicId =
                                    topic.Id,

                                IsEnrolled =
                                    isEnrolled,

                                AttendanceStatus =
                                    "EMPTY",

                                Grade =
                                    null
                            };

                        await _unitOfWork
                            .TopicEvaluations
                            .AddAsync(
                                evaluation
                            );
                    }
                    else
                    {
                        /*
                         * Preservamos Status y Grade.
                         */
                        evaluation.IsEnrolled =
                            isEnrolled;
                    }
                }
            }

            /*
             * Eliminamos participantes quitados de la matriz.
             *
             * Ya comprobamos arriba que no tengan evidencia.
             */
            foreach (
                var attendee
                in attendeesToRemove
            )
            {
                _unitOfWork
                    .TopicEvaluations
                    .DeleteRange(
                        attendee.Evaluations
                    );

                _unitOfWork
                    .EventAttendees
                    .Delete(
                        attendee
                    );
            }

            await _unitOfWork
                .SaveChangesAsync(
                    cancellationToken
                );

            return new UpdateAttendeesResult(
                UpdateAttendeesStatus.Updated,
                "Participantes actualizados correctamente."
            );
        }

        private static bool HasCapturedInformation(
            EventAttendee attendee
        )
        {
            if (
                !string.IsNullOrWhiteSpace(
                    attendee.ParticipantSignatureUrl
                )
            )
            {
                return true;
            }

            return attendee.Evaluations.Any(
                evaluation =>
                    evaluation.Grade.HasValue ||
                    HasCapturedAttendanceStatus(
                        evaluation.AttendanceStatus
                    )
            );
        }

        private static bool HasCapturedAttendanceStatus(
            string? status
        )
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return false;
            }

            return
                !status.Equals(
                    "EMPTY",
                    StringComparison.OrdinalIgnoreCase
                ) &&
                !status.Equals(
                    "PENDING",
                    StringComparison.OrdinalIgnoreCase
                );
        }

        private static UpdateAttendeesResult Invalid(
            string message
        )
        {
            return new UpdateAttendeesResult(
                UpdateAttendeesStatus.InvalidRequest,
                message
            );
        }
    }
}