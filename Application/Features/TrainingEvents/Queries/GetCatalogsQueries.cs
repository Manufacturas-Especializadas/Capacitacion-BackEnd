using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingEvents.Queries
{
    public record GetRoomsQuery() : IRequest<List<CatalogItemDto>>;

    public class GetRoomsHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetRoomsQuery, List<CatalogItemDto>>
    {
        public async Task<List<CatalogItemDto>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
        {
            var rooms = await unitOfWork.TrainingRooms.GetAllAsync();

            return rooms.Select(r => new CatalogItemDto
            {
                Id = r.Id,
                Name = r.RoomName
            })
            .OrderBy(r => r.Id)
            .ToList();
        }
    }

    public record GetLinesQuery() : IRequest<List<CatalogItemDto>>;
    
    public class GetLinesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetLinesQuery, List<CatalogItemDto>>
    {
        public async Task<List<CatalogItemDto>> Handle(GetLinesQuery request, CancellationToken cancellationToken)
        {
            var lines = await unitOfWork.ProductionLines.GetAllAsync();
            return lines.Select(l => new CatalogItemDto
            {
                Id = l.Id,
                Name = l.LineName
            })
            .OrderBy(r => r.Id)
            .ToList();
        }
    }

    public record GetTutors(): IRequest<List<CatalogItemDto>>;

    public class GetTutorsHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetTutors, List<CatalogItemDto>>
    {
        public async Task<List<CatalogItemDto>> Handle(GetTutors request, CancellationToken cancellationToken)
        {
            var tutors = await unitOfWork.Tutors.GetAllAsync();
            return tutors.Select(t => new CatalogItemDto
            {
                Id = t.Id,
                Name = t.TutorName
            })
            .OrderBy(r => r.Id)
            .ToList();
        }
    }
}