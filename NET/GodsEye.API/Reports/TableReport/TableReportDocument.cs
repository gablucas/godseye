using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data.Common;

namespace GodsEye.API.Reports.TableReport
{
    public class TableReportDocument<T> : IDocument
    {
        public string ReportTitle { get; set; } = String.Empty;
        public List<T> Data { get; }

        private List<string> TableHeaders { get; set; } = new();

        private List<Func<T, string>> CellValues { get; set; } = new();

        private DateTime EmissionDate { get; set; }

        public TableReportDocument(string reportTitle, List<T> data)
        {
            ReportTitle = reportTitle;
            Data = data;
            EmissionDate = DateTime.Now;
        }

        public void Teste(string tableHeader, Func<T, string> teste)
        {
            TableHeaders.Add(tableHeader);
            CellValues.Add(teste);
        }


        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(20);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                //page.Footer().Height(50).Background(Colors.Grey.Lighten1);
            });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item()
                        .Text(ReportTitle)
                        .FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

                    column.Item().Text(text =>
                    {
                        text.Span("Data da emissão: ").SemiBold();
                        text.Span(EmissionDate.ToString());
                    });

                    //column.Item().Text(text =>
                    //{
                    //    text.Span("Due date: ").SemiBold();
                    //    text.Span($"");
                    //});
                });

                //row.ConstantItem(100).Height(50).Placeholder();
            });
        }


        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(20).Column(column =>
            {
                column.Spacing(5);

                column.Item().Element(ComposeTable);
            });
        }

        void ComposeTable(IContainer container)
        {
            container
                .Border(0.5f)
                .BorderColor("#F7F7F7")
                .CornerRadius(7)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        for (var i = 0; i < CellValues.Count; i++)
                            columns.RelativeColumn();
                    });

                    table.Header(header =>
                    {
                        foreach (var tableHeader in TableHeaders)
                        {
                            header.Cell()
                                .Background("#F7F7F7")
                                .PaddingVertical(8)
                                .PaddingHorizontal(12)
                                .Text(tableHeader)
                                .FontSize(11)
                                .FontColor("#5F5E5A")
                                .SemiBold();
                        }
                    });

                    foreach (var data in Data)
                    {
                        for (var i = 0; i < CellValues.Count; i++)
                        {
                            table.Cell()
                            .Element(CellStyle)
                            //.Background(isEven ? "#FFFFFF" : "#F8F8F6")
                            .Background("#FFFFFF")
                            .PaddingHorizontal(12)
                            .Text(CellValues[i](data))
                            .FontSize(10)
                            .FontColor("#2C2C2A");

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(0.2f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(4);
                            }
                        }
                    }
                });
        }
    }
}
