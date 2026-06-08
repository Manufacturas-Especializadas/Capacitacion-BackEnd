using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TrainingEvents.Queries
{
    public class GetAllWelderEvaluationsQuery() : IRequest<List<WelderEvaluationListDto>>;

    public class GetAllWelderEvaluationsQueryHandler(IUnitOfWork unitOfWork)
        : IRequestHandler<GetAllWelderEvaluationsQuery, List<WelderEvaluationListDto>>
    {
        public async Task<List<WelderEvaluationListDto>> Handle(GetAllWelderEvaluationsQuery request, CancellationToken cancellationToken)
        {
            var list = await unitOfWork.WelderEvaluations.GetAllWithRelationsAsync();

            return list.Select(e => new WelderEvaluationListDto
            {
                Id = e.Id,
                EmployeeNumber = e.Employee?.EmployeeNumber ?? "N/A",
                EmployeeName = e.Employee?.Name ?? "Sin nombre",
                EvaluationDate = e.EvaluationDate,
                FinalAverage = e.FinalAverage,
                MasteryLevel = e.MasteryLevel
            }).ToList();
        }
    }
}