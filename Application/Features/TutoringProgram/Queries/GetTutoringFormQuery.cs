using Application.DTOs;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.TutoringProgram.Queries
{
    public record GetTutoringFormQuery() : IRequest<IEnumerable<FormSectionDto>>;

    public class GetTutoringFormQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetTutoringFormQuery, IEnumerable<FormSectionDto>>
    {
        public async Task<IEnumerable<FormSectionDto>> Handle(GetTutoringFormQuery request, CancellationToken cancellationToken)
        {
            var sections = await unitOfWork.Sections.GetAllAsync();
            var questions = await unitOfWork.Questions.GetAllAsync();
            var questionTypes = await unitOfWork.QuestionTypes.GetAllAsync();
            var questionOptions = await unitOfWork.QuestionOptions.GetAllAsync();
            var optionsCatalogs = await unitOfWork.OptionsCatalogs.GetAllAsync();


            var formTemplate = sections
                    .OrderBy(s => s.DisplayOrder)
                    .Select(s => new FormSectionDto
                    {
                        Id = s.Id,
                        SectionName = s.SectionName,
                        Questions = questions
                            .Where(q => q.SectionId == s.Id)
                            .OrderBy(q => q.DisplayOrder)
                            .Select(q => new FormQuestionDto
                            {
                                Id = q.Id,
                                QuestionText = q.QuestionText,
                                QuestionTypeId = q.QuestionTypeId,
                                QuestionTypeName = questionTypes.FirstOrDefault(qt => qt.Id == q.QuestionTypeId)?.TypeName ?? "",
                                DisplayOrder = q.DisplayOrder,
                                IsRequired = q.IsRequired,
                                MaxRating = q.MaxRating,
                                ParentQuestionId = q.ParentQuestionId,
                                ShowWhenOptionId = q.ShowWhenOptionId,
                                Options = questionOptions
                                    .Where(qo => qo.QuestionId == q.Id)
                                    .OrderBy(qo => qo.DisplayOrder)
                                    .Select(qo => new FormOptionDto
                                    {
                                        OptionId = qo.OptionId,
                                        OptionText = optionsCatalogs.FirstOrDefault(oc => oc.Id == qo.OptionId)?.OptionText ?? "",
                                        DisplayOrder = qo.DisplayOrder
                                    }).ToList()
                            }).ToList()
                    });

            return formTemplate;
        }
    }
}