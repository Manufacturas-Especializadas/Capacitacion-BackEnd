using Domain.Interfaces;
using Application.DTOs;
using MediatR;

namespace Application.Features.TrainingTopics.Queries
{
    public record GetAllTrainingTopicsQuery : IRequest<IEnumerable<TrainingTopicDto>>;

    public class GetAllTrainingTopicsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllTrainingTopicsQuery, IEnumerable<TrainingTopicDto>>
    {
        public async Task<IEnumerable<TrainingTopicDto>> Handle(GetAllTrainingTopicsQuery request, CancellationToken cancellationToken)
        {
            var topics = await unitOfWork.TrainingTopics.GetAllAsync();

            return topics.Select(t => new TrainingTopicDto
            {
                Id = t.Id,
                TrainingType = t.TrainingType,
                TopicCode = t.TopicCode,
                TopicName = t.TopicName
            }).OrderBy(t => t.TrainingType).ThenBy(t => t.TopicCode);
        }
    }
}