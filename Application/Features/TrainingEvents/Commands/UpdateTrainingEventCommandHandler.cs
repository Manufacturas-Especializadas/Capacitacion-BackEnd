using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingEvents.Commands
{
    public enum UpdateTrainingEventStatus
    {
        Updated,
        InvalidRequest,
        NotFound
    }

    public sealed record UpdateTrainingEventResult(
        UpdateTrainingEventStatus Status,
        string Message
    );

    public record UpdateTrainingEventCommand(
        int EventId,
        UpdateTrainingEventDto Data
    ) : IRequest<UpdateTrainingEventResult>;

    public class UpdateTrainingEventCommandHandler(
        IUnitOfWork unitOfWork
    ) : IRequestHandler<
        UpdateTrainingEventCommand,
        UpdateTrainingEventResult
    >
    {
        private readonly IUnitOfWork _unitOfWork =
            unitOfWork;

        public async Task<UpdateTrainingEventResult> Handle(
            UpdateTrainingEventCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.EventId <= 0)
            {
                return Invalid(
                    "El identificador del evento no es válido."
                );
            }

            var data = request.Data;

            if (string.IsNullOrWhiteSpace(data.CourseName))
            {
                return Invalid(
                    "El nombre del curso es obligatorio."
                );
            }

            if (string.IsNullOrWhiteSpace(data.InstructorName))
            {
                return Invalid(
                    "El nombre del instructor es obligatorio."
                );
            }

            if (
                data.EvaluationTopics.Count == 0 ||
                data.EvaluationTopics.Count > 5
            )
            {
                return Invalid(
                    "El evento debe tener entre 1 y 5 temas."
                );
            }

            foreach (var topic in data.EvaluationTopics)
            {
                if (string.IsNullOrWhiteSpace(topic.Name))
                {
                    return Invalid(
                        "Todos los temas deben tener nombre."
                    );
                }

                if (topic.EndTime <= topic.StartTime)
                {
                    return Invalid(
                        $"La hora final debe ser mayor que la hora inicial " +
                        $"en el tema '{topic.Name}'."
                    );
                }
            }

            if (data.DateTo < data.DateFrom)
            {
                return Invalid(
                    "La fecha final no puede ser menor que la fecha inicial."
                );
            }

            var room =
                await _unitOfWork
                    .TrainingRooms
                    .GetByIdAsync(
                        data.RoomId
                    );

            if (room is null)
            {
                return Invalid(
                    "La sala seleccionada no existe."
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
                return new UpdateTrainingEventResult(
                    UpdateTrainingEventStatus.NotFound,
                    $"No se encontró el evento con ID {request.EventId}."
                );
            }

            var existingTopics =
                trainingEvent.Topics
                    .OrderBy(topic => topic.TopicOrder)
                    .ToList();

            /*
             * IDs que el frontend afirma que ya existen.
             */
            var submittedIds =
                data.EvaluationTopics
                    .Where(topic =>
                        topic.Id.HasValue
                    )
                    .Select(topic =>
                        topic.Id!.Value
                    )
                    .ToList();

            if (
                submittedIds.Count !=
                submittedIds.Distinct().Count()
            )
            {
                return Invalid(
                    "No se puede enviar el mismo tema más de una vez."
                );
            }

            var existingIds =
                existingTopics
                    .Select(topic => topic.Id)
                    .ToHashSet();

            /*
             * Evitamos que alguien intente editar
             * un tema perteneciente a otro evento.
             */
            if (
                submittedIds.Any(id =>
                    !existingIds.Contains(id)
                )
            )
            {
                return Invalid(
                    "Uno de los temas enviados no pertenece al evento."
                );
            }

            var submittedIdSet =
                submittedIds.ToHashSet();

            var topicsToRemove =
                existingTopics
                    .Where(topic =>
                        !submittedIdSet.Contains(
                            topic.Id
                        )
                    )
                    .ToList();

            /*
             * Protegemos evidencia histórica.
             */
            foreach (var topic in topicsToRemove)
            {
                var evaluations =
                    trainingEvent.Attendees
                        .SelectMany(attendee =>
                            attendee.Evaluations
                        )
                        .Where(evaluation =>
                            evaluation.TopicId ==
                            topic.Id
                        )
                        .ToList();

                if (
                    evaluations.Any(
                        HasCapturedInformation
                    )
                )
                {
                    return Invalid(
                        $"No se puede eliminar el tema '{topic.TopicName}' " +
                        "porque ya tiene asistencia o calificaciones capturadas."
                    );
                }
            }

            /*
             * Datos generales.
             */
            trainingEvent.CourseName =
                data.CourseName.Trim();

            trainingEvent.InstructorName =
                data.InstructorName.Trim();

            trainingEvent.RoomId =
                data.RoomId;

            trainingEvent.DateFrom =
                data.DateFrom;

            trainingEvent.DateTo =
                data.DateTo;

            /*
             * Actualizamos temas existentes
             * y agregamos nuevos.
             */
            for (
                var index = 0;
                index < data.EvaluationTopics.Count;
                index++
            )
            {
                var requestedTopic =
                    data.EvaluationTopics[index];

                if (requestedTopic.Id.HasValue)
                {
                    var existingTopic =
                        existingTopics.First(
                            topic =>
                                topic.Id ==
                                requestedTopic.Id.Value
                        );

                    existingTopic.TopicName =
                        requestedTopic.Name.Trim();

                    existingTopic.TopicDate =
                        requestedTopic.Date;

                    existingTopic.StartTime =
                        requestedTopic.StartTime;

                    existingTopic.EndTime =
                        requestedTopic.EndTime;

                    existingTopic.TopicOrder =
                        index;

                    continue;
                }

                /*
                 * Tema nuevo.
                 */
                var newTopic =
                    new EventTopic
                    {
                        EventId =
                            trainingEvent.Id,

                        TopicName =
                            requestedTopic.Name.Trim(),

                        TopicDate =
                            requestedTopic.Date,

                        StartTime =
                            requestedTopic.StartTime,

                        EndTime =
                            requestedTopic.EndTime,

                        TopicOrder =
                            index
                    };

                await _unitOfWork
                    .EventTopics
                    .AddAsync(
                        newTopic
                    );

                /*
                 * Todo participante existente necesita
                 * una celda para el nuevo tema.
                 *
                 * Empieza sin inscripción y después
                 * EnrollmentMatrix permitirá marcarla.
                 */
                foreach (
                    var attendee
                    in trainingEvent.Attendees
                )
                {
                    var evaluation =
                        new TopicEvaluation
                        {
                            AttendeeId =
                                attendee.Id,

                            Topic =
                                newTopic,

                            IsEnrolled =
                                false,

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
            }

            /*
             * Temas eliminados.
             */
            foreach (
                var topic
                in topicsToRemove
            )
            {
                var evaluations =
                    trainingEvent.Attendees
                        .SelectMany(attendee =>
                            attendee.Evaluations
                        )
                        .Where(evaluation =>
                            evaluation.TopicId ==
                            topic.Id
                        )
                        .ToList();

                if (evaluations.Count > 0)
                {
                    _unitOfWork
                        .TopicEvaluations
                        .DeleteRange(
                            evaluations
                        );
                }

                _unitOfWork
                    .EventTopics
                    .Delete(
                        topic
                    );
            }

            _unitOfWork
                .TrainingEvents
                .Update(
                    trainingEvent
                );

            await _unitOfWork
                .SaveChangesAsync(
                    cancellationToken
                );

            return new UpdateTrainingEventResult(
                UpdateTrainingEventStatus.Updated,
                "Evento actualizado correctamente."
            );
        }

        private static bool HasCapturedInformation(
            TopicEvaluation evaluation
        )
        {
            if (evaluation.Grade.HasValue)
            {
                return true;
            }

            if (
                string.IsNullOrWhiteSpace(
                    evaluation.AttendanceStatus
                )
            )
            {
                return false;
            }

            return
                !evaluation.AttendanceStatus.Equals(
                    "EMPTY",
                    StringComparison.OrdinalIgnoreCase
                ) &&
                !evaluation.AttendanceStatus.Equals(
                    "PENDING",
                    StringComparison.OrdinalIgnoreCase
                );
        }

        private static UpdateTrainingEventResult Invalid(
            string message
        )
        {
            return new UpdateTrainingEventResult(
                UpdateTrainingEventStatus.InvalidRequest,
                message
            );
        }
    }
}