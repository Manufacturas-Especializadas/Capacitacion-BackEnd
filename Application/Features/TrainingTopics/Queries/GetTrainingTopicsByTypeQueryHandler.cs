using Domain.Interfaces;
using Application.DTOs;
using MediatR;

namespace Application.Features.TrainingTopics.Queries
{
    public record GetTrainingTopicsByTypeQuery(string TrainingType) : IRequest<IEnumerable<TrainingTopicDto>>;

    public class GetTrainingTopicsByTypeQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetTrainingTopicsByTypeQuery, IEnumerable<TrainingTopicDto>>
    {
        public async Task<IEnumerable<TrainingTopicDto>> Handle(GetTrainingTopicsByTypeQuery request, CancellationToken cancellationToken)
        {
            var allTopics = await unitOfWork.TrainingTopics.GetAllAsync();

            return allTopics
                .Where(t => t.TrainingType.ToUpper() == request.TrainingType.ToUpper())
                .Select(t => new TrainingTopicDto
                {
                    Id = t.Id,
                    TrainingType = t.TrainingType,
                    TopicCode = t.TopicCode,
                    TopicName = t.TopicName
                }).OrderBy(t => t.TopicCode);
        }
    }
}