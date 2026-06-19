using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingEvents.Commands
{
    public record AssignAttendeesCommand(AssignAttendeesDto Data) : IRequest<bool>;

    public class AssignAttendeesCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AssignAttendeesCommand, bool> 
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<bool> Handle(AssignAttendeesCommand request, CancellationToken cancellationToken)
        {
            var data = request.Data;

            var trainingEvent = await _unitOfWork.TrainingEvents.GetEventWithDetailAsync(data.EventId);

            if(trainingEvent == null)
            {
                throw new ArgumentException($"No se encontró el evento con ID {data.EventId}");
            }

            var topics = trainingEvent.Topics.OrderBy(t => t.TopicOrder).ToList();

            foreach(var row in data.Attendees)
            {
                var employee = await _unitOfWork.Employees.GetByEmployeeNumberAsync(row.EmployeeNumber);

                if (employee == null)
                {
                    var line = await _unitOfWork.ProductionLines.GetByNameAsync(row.LineName);

                    if (line == null)
                    {
                        throw new ArgumentException($"La linea {row.LineName} no existe en el catálogo");
                    }

                    employee = new Domain.Entities.Employee
                    {
                        EmployeeNumber = row.EmployeeNumber,
                        Name = row.Name,
                        LineId = line.Id,
                    };

                    await _unitOfWork.Employees.AddAsync(employee);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                var attendee = new EventAttendee
                {
                    EventId = trainingEvent.Id,
                    EmployeeId = employee.Id,
                };

                await _unitOfWork.EventAttendees.AddAsync(attendee);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                for (int i = 0; i < topics.Count; i++)
                {
                    bool isEnrolled = row.Enrollments.ElementAtOrDefault(i);

                    var evaluation = new TopicEvaluation
                    {
                        AttendeeId = attendee.Id,
                        TopicId = topics[i].Id,
                        IsEnrolled = isEnrolled,
                        AttendanceStatus = "EMPTY",
                        Grade = null
                    };

                    await _unitOfWork.TopicEvaluations.AddAsync(evaluation);
                }
            }

            trainingEvent.Status = "Programado";
            _unitOfWork.TrainingEvents.Update(trainingEvent);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}