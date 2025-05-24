using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System; // Thêm System để dùng DateTime và Convert
using Microsoft.Extensions.Logging; // Để inject logger nếu muốn

namespace PBL3.Models.ViewModel
{
    public class TicketPdfDocument : IDocument
    {
        private readonly TicketPdfModel _model;
        private readonly ILogger<TicketPdfDocument> _logger; // Tùy chọn

        // Constructor có thể nhận logger
        public TicketPdfDocument(TicketPdfModel model, ILogger<TicketPdfDocument> logger = null)
        {
            _model = model;
            _logger = logger;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A5.Portrait()); // Sử dụng A5 dọc
                page.Margin(1.5f, Unit.Centimetre); // Điều chỉnh margin
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Calibri)); // Font và size nhỏ hơn cho A5

                page.Header()
                    .AlignCenter()
                    .PaddingBottom(5, Unit.Millimetre) // Giảm padding
                    .Text("VÉ GỬI XE SINH VIÊN") // Tiêu đề rõ ràng hơn
                    .Bold().FontSize(16).FontColor(Colors.Blue.Darken2); // Giảm size tiêu đề

                page.Content()
                    .PaddingVertical(5, Unit.Millimetre)
                    .Column(column =>
                    {
                        column.Spacing(8); // Giảm khoảng cách giữa các items

                        // Helper để tạo dòng thông tin cho Grid
                        Action<GridDescriptor, string, string> AddRow = (grid, label, value) =>
                        {
                            grid.Item(4).AlignRight().PaddingRight(5).Text(label).SemiBold();
                            grid.Item(8).Text(value ?? "N/A");
                        };

                        column.Item().Grid(grid =>
                        {
                            grid.Columns(12); // Chia thành 12 cột
                            grid.VerticalSpacing(3); // Khoảng cách giữa các dòng trong grid

                            AddRow(grid, "Họ tên:", _model.HoTen);
                            AddRow(grid, "MSSV:", _model.MSSV);
                            AddRow(grid, "Biển số xe:", _model.BienSoXe);
                            AddRow(grid, "Khu vực gửi xe:", _model.SlotName); // Đổi "Chỗ đậu xe" thành "Khu vực gửi xe"
                            AddRow(grid, "Ngày đăng ký:", _model.NgayDangKy.ToString("dd/MM/yyyy"));
                            AddRow(grid, "Ngày hết hạn:", _model.NgayHetHan.ToString("dd/MM/yyyy HH:mm")); // Thêm giờ phút cho ngày hết hạn
                            AddRow(grid, "Giá vé:", _model.Price.ToString("N0") + " VNĐ");
                        });


                        // Mã QR Code (MỚI THÊM)
                        if (!string.IsNullOrEmpty(_model.QRCode))
                        {
                            try
                            {
                                byte[] qrCodeBytes = Convert.FromBase64String(_model.QRCode);
                                column.Item()
                                    .PaddingTop(1, Unit.Centimetre) // Khoảng cách trước QR
                                    .AlignCenter()
                                    .Width(4, Unit.Centimetre) // Kích thước QR
                                    .Height(4, Unit.Centimetre)
                                    .Image(qrCodeBytes)
                                    .FitArea();
                            }
                            catch (FormatException ex)
                            {
                                _logger?.LogError(ex, "Lỗi chuyển đổi Base64 cho QR code: {QRCodeString}", _model.QRCode);
                                column.Item().Text("Lỗi hiển thị QR Code.").FontColor(Colors.Red.Medium).AlignCenter();
                            }
                        }
                        else
                        {
                            column.Item().PaddingTop(1, Unit.Centimetre).Text("Không có mã QR.").AlignCenter();
                        }
                        column.Item().PaddingTop(1, Unit.Centimetre)
                                .Text("Lưu ý: Xuất trình vé khi ra vào cổng. Vé hợp lệ cho biển số đã đăng ký.")
                                .Italic().AlignCenter().FontSize(8);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Ngày in: ").SemiBold().FontSize(8);
                        x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")).FontSize(8); // Thêm giây
                    });
            });
        }
    }
}