using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

QuestPDF.Settings.License = LicenseType.Community;

Document.Create(container =>
{
    container.Page(page =>
    {
        page.Size(PageSizes.A4);
        page.Margin(2, Unit.Centimetre);

        page.Header()
            .Text("Relatório de Teste")
            .FontSize(20)
            .Bold();

        page.Content()
            .PaddingTop(20)
            .Text("Conteúdo do relatório aqui!");

        page.Footer()
            .Text(text =>
            {
                text.Span("Página ");
                text.CurrentPageNumber();
            });
    });
})
.ShowInPreviewer();