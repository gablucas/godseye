using GodsEye.API.Interfaces;
using GodsEye.API.Reports.TableReport;
using GodsEye.Shared.Extensions;
using GodsEye.Shared.Response.Compliance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;

namespace GodsEye.API.Features.Compliance.Violation
{
    public sealed record GetComplianceViolationPDFReportCommand() : IRequest<byte[]>;

    internal sealed class GetComplianceViolationPDFReportHandler(IComplianceViolationQuery complianceViolationQuery) : IRequestHandler<GetComplianceViolationPDFReportCommand, byte[]>
    {
        public async Task<byte[]> Handle(GetComplianceViolationPDFReportCommand request, CancellationToken cancellationToken)
        {
            var result = (await complianceViolationQuery.GetAllComplianceViolationQuery(1, 10, cancellationToken)).ToList();

            if (result is null)
                throw new InvalidOperationException("Não foi possível encontrar o compliance");

            var PDFReport = new TableReportDocument<ComplianceViolationResponse>("Relatório de violações", result);

            PDFReport.Teste("Pessoa", x => x.PersonName);
            PDFReport.Teste("Política", x => x.PolicyName);
            PDFReport.Teste("Tipo de politica", x => x.PolicyType.GetDescription());
            PDFReport.Teste("Tipo de violação", x => x.ViolationType.GetDescription());
            PDFReport.Teste("Setor", x => x.SectorName);
            PDFReport.Teste("Data do registro", x => x.CreatedAt.ToString());

            var PDF = PDFReport.GeneratePdf();

            return PDF;
        }
    }

    public class ComplianceViolationPDFReportController : IEndpoint
    {
        public void MapEndpoint(WebApplication app)
        {
            app.MapGet("/api/compliance/violation/report", Handle);
        }

        private static async Task<IResult> Handle(
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var response = await mediator.Send(new GetComplianceViolationPDFReportCommand(), cancellationToken);
            return Results.File(response, "application/pdf", "relatorio.pdf");
        }
    }
}
