
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
namespace PBL3.Models.ViewModel
{
    public class TicketPdfDocument : IDocument
    {
        private readonly TicketPdfModel _model;

        public TicketPdfDocument(TicketPdfModel model)
        {
            _model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A5);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .AlignCenter()
                    .Text("VÉ XE")
                    .Bold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Spacing(20);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Họ tên:").SemiBold();
                            row.RelativeItem(2).Text(_model.HoTen);
                        });

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("MSSV:").SemiBold();
                            row.RelativeItem(2).Text(_model.MSSV);
                        });

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Biển số xe:").SemiBold();
                            row.RelativeItem(2).Text(_model.BienSoXe);
                        });

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Chỗ đậu xe:").SemiBold();
                            row.RelativeItem(2).Text(_model.SlotName);
                        });

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Ngày đăng ký:").SemiBold();
                            row.RelativeItem(2).Text(_model.NgayDangKy.ToString("dd/MM/yyyy"));
                        });

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Ngày hết hạn:").SemiBold();
                            row.RelativeItem(2).Text(_model.NgayHetHan.ToString("dd/MM/yyyy"));
                        });

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text("Giá vé:").SemiBold();
                            row.RelativeItem(2).Text(_model.Price.ToString("N0") + " VNĐ");
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Ngày in: ").SemiBold();
                        x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                    });
            });
        }
    }
}
