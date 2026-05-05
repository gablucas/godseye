using GodsEye.API.Features.Compliance.Shared.Query;
using GodsEye.API.Features.Compliance.Violation;
using GodsEye.API.Interfaces;
using GodsEye.API.Reports.TableReport;
using GodsEye.Shared.Enums;
using GodsEye.Shared.Extensions;
using GodsEye.Shared.Response.Compliance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;

namespace GodsEye.API.Features.Compliance.SectorTransition.GetReportPDF
{
    public sealed record GetSectorTransitionViolationReportPDFCommand() : IRequest<byte[]>;

    internal sealed class GetSectorTransitionViolationReportPDFHandler(IComplianceViolationQuery complianceViolationQuery, ISectorTransitionQuery sectorTransitionQuery) : IRequestHandler<GetSectorTransitionViolationReportPDFCommand, byte[]>
    {
        public async Task<byte[]> Handle(GetSectorTransitionViolationReportPDFCommand request, CancellationToken cancellationToken)
        {
            var result = (await complianceViolationQuery.GetAllComplianceViolationQuery(1, 10, cancellationToken)).ToList();
            var sectorTransitionRule = await sectorTransitionQuery.GetAll(cancellationToken);

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o compliance");

            var PDFReport = new TableReportDocument<ComplianceViolationResponse>("Relatório de violações", result);

            PDFReport.Teste("Pessoa", x => x.PersonName);
            PDFReport.Teste("Política", x => x.PolicyName);
            PDFReport.Teste("Tipo de violação", x => x.ViolationType.GetDescription());
            PDFReport.Teste("Setor", x => x.SectorName);


            PDFReport.Teste("Detalhes", x =>
            {
                if (x.ViolationType == ComplianceViolationEnum.BELOW_MINIMUM_TIME)
                {
                    if (x.EnteredAt != null && x.ExitedAt != null)
                    {
                        var duration = x.ExitedAt.Value - x.EnteredAt.Value;
                        return $"{(int)duration.TotalHours}h {duration.Minutes}m {duration.Seconds}s";
                    }
                }
                else if (x.ViolationType == ComplianceViolationEnum.EXCEEDED_ALLOWED_TIME)
                {
                    if (x.EnteredAt != null && x.ExitedAt != null)
                    {
                        var duration = x.ExitedAt.Value - x.EnteredAt.Value;
                        var maxTimeSector = sectorTransitionRule.First(str => str.PolicyId == x.PolicyId).Rules.First(r => r.SectorId == x.SectorId).MaxTime;

                        var exceededTime = duration - TimeSpan.FromMinutes(maxTimeSector ?? 0);

                        return exceededTime.ToString();
                    }
                }
                else if (x.ViolationType == ComplianceViolationEnum.DID_NOT_ENTER_NEXT_SECTOR_IN_TIME)
                {
                    var rules = sectorTransitionRule.First(x => x.PolicyId == x.PolicyId).Rules;
                    var logSectorIndex = rules.First(x => x.SectorId == x.SectorId).OrderIndex;
                    var nextSector = rules.First(x => x.OrderIndex == logSectorIndex + 1).SectorName;

                    return "Sala de descanso";
                }


                return "Sem registro";
            });


            PDFReport.Teste("Data entrada", x => x.EnteredAt.ToString());
            PDFReport.Teste("Data saída", x => x.ExitedAt.ToString());
            


            var PDF = PDFReport.GeneratePdf();

            return PDF;
        }
    }

    public class GetSectorTransitionViolationReportPDFController : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/compliance/violation/report/sector-transition", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetSectorTransitionViolationReportPDFCommand(), cancellationToken);
            return Results.File(response, "application/pdf", "relatorio.pdf");
        }
    }
}
