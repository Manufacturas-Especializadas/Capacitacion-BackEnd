using MediatR;

namespace Application.Features.TrainingEvents.Commands
{
    public class DeleteTrainingEventCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public DeleteTrainingEventCommand(int id)
        {
            Id = id;
        }
    }
}
