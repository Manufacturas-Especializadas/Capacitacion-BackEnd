using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Application.Features.TrainingReports.Commands
{
    public record DeleteTrainingReportCommand(
        int ReportId
    ) : IRequest<bool>;

    public class DeleteTrainingReportCommandHandler(IUnitOfWork unitOfWork): IRequestHandler<DeleteTrainingReportCommand, bool>
    {
        public async Task<bool> Handle(DeleteTrainingReportCommand request, CancellationToken cancellationToken )
        {
            if (request.ReportId <= 0)
            {
                return false;
            }

            var wasFound = await unitOfWork
                .TrainingReports
                .DeleteWithDetailsAsync(
                    request.ReportId,
                    cancellationToken
                );

            if (!wasFound)
            {
                return false;
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

}
