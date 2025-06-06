using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PBL3.Data;
using PBL3.Models; // Đảm bảo Models được using
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PBL3.Controllers
{
    [Authorize(Roles = "Staff")]
    public class QRCodeController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ILogger<QRCodeController> _logger;

        public QRCodeController(AppDBContext context, ILogger<QRCodeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /QRCode/Scan
        public IActionResult Scan()
        {
            _logger.LogInformation("QRCodeController GET Scan: Displaying QR scan page.");
            return View();
        }

        // Action để tìm vé theo MSSV (dùng cho nhập tay)
        [HttpPost]
        public async Task<IActionResult> FindAndProcessTicketByMSSV([FromBody] ManualCheckinRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.MSSV))
            {
                _logger.LogWarning("FindAndProcessTicketByMSSV: MSSV is null or empty.");
                return BadRequest(new { success = false, message = "Vui lòng nhập MSSV." });
            }

            _logger.LogInformation("FindAndProcessTicketByMSSV: Searching for active ticket for MSSV {MSSV}", request.MSSV);

            // Với TPH, _context.Students sẽ tự động lọc AppUser có Discriminator là "Student"
            var student = await _context.Users.OfType<Student>() // Cách tường minh hơn để chỉ rõ là query từ Users và lọc Student
                                      .FirstOrDefaultAsync(s => s.MSSV == request.MSSV);


            if (student == null)
            {
                _logger.LogWarning("FindAndProcessTicketByMSSV: Student with MSSV {MSSV} not found.", request.MSSV);
                return NotFound(new { success = false, message = $"Không tìm thấy sinh viên với MSSV: {request.MSSV}." });
            }

            // Tìm vé còn hạn của sinh viên này
            // Ưu tiên vé đang ở trong bãi (có ParkingSlotId) để check-out,
            // hoặc vé còn hạn chưa vào bãi để check-in.
            var ticket = await _context.Tickets
                .Include(t => t.Student) 
                .Include(t => t.ParkingSlot)
                .Where(t => t.StudentId == student.Id && t.NgayHetHan.Date >= DateTime.Now.Date) // Vé còn hạn
                .OrderByDescending(t => t.ParkingSlotId != null) // Vé đang trong bãi ưu tiên (cho check-out)
                .ThenByDescending(t => t.NgayHetHan)             // Sau đó là vé hết hạn muộn nhất
                .FirstOrDefaultAsync();

            if (ticket == null)
            {
                _logger.LogWarning("FindAndProcessTicketByMSSV: No active/suitable ticket found for student {StudentName} (MSSV: {MSSV}).", student.HoTen, request.MSSV);
                return NotFound(new { success = false, message = $"Sinh viên {student.HoTen} (MSSV: {request.MSSV}) không có vé nào còn hạn hoặc phù hợp để check-in/out." });
            }

            _logger.LogInformation("FindAndProcessTicketByMSSV: Found ticket ID {TicketId} for MSSV {MSSV}. Proceeding to process.", ticket.ID_Ticket, request.MSSV);
            return await ProcessTicketInternal(ticket.ID_Ticket.ToString());
        }

        // POST: /QRCode/ProcessQRCode
        [HttpPost]
        public async Task<IActionResult> ProcessQRCode([FromBody] QRCodeScanRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.TicketIdContent))
            {
                _logger.LogWarning("ProcessQRCode POST: Received null or empty QR content.");
                return BadRequest(new { success = false, message = "Dữ liệu QR không hợp lệ (trống)." });
            }
            _logger.LogInformation("ProcessQRCode POST: Received QR content: {QRContent}", request.TicketIdContent);
            return await ProcessTicketInternal(request.TicketIdContent);
        }

        // Hàm xử lý nội bộ, dùng chung cho cả quét QR và nhập tay
        private async Task<IActionResult> ProcessTicketInternal(string ticketIdContent)
        {
            if (!int.TryParse(ticketIdContent, out int ticketId))
            {
                _logger.LogWarning("ProcessTicketInternal: Invalid TicketID format. Expected integer, got: {TicketIdContent}", ticketIdContent);
                return BadRequest(new { success = false, message = "Mã vé không đúng định dạng." });
            }

            var ticket = await _context.Tickets
                .Include(t => t.Student)    // t.Student ở đây là AppUser
                .Include(t => t.ParkingSlot)
                .FirstOrDefaultAsync(t => t.ID_Ticket == ticketId);

            if (ticket == null)
            {
                _logger.LogWarning("ProcessTicketInternal: Ticket with ID {TicketId} not found.", ticketId);
                return NotFound(new { success = false, message = $"Không tìm thấy vé xe với ID: {ticketId}." });
            }

            // Với TPH, ticket.Student là AppUser. Cần ép kiểu để lấy thuộc tính của Student.
            var studentDetails = ticket.Student as Student;
            var studentInfoForResponse = new
            {
                HoTen = ticket.Student?.HoTen ?? "N/A", // HoTen là của AppUser
                MSSV = studentDetails?.MSSV ?? "N/A",   // MSSV là của Student (sau khi ép kiểu)
                Lop = studentDetails?.Lop ?? "N/A"      // Lop là của Student (sau khi ép kiểu)
            };

            if (ticket.Student == null) // Kiểm tra thêm nếu StudentId trong Ticket không hợp lệ
            {
                _logger.LogError("ProcessTicketInternal: Ticket ID {TicketId} has an invalid StudentId or Student record is missing. StudentId: {StudentId}", ticket.ID_Ticket, ticket.StudentId);
                return StatusCode(500, new { success = false, message = "Lỗi dữ liệu: Vé này không liên kết với sinh viên hợp lệ." });
            }


            if (ticket.NgayHetHan.Date < DateTime.Now.Date)
            {
                _logger.LogInformation("ProcessTicketInternal: Ticket ID {TicketId} for student {StudentMSSV} expired on {ExpiryDate}.",
                                     ticket.ID_Ticket, studentInfoForResponse.MSSV, ticket.NgayHetHan.ToString("dd/MM/yyyy"));
                return Json(new
                { // Trả về JSON để client xử lý
                    success = false,
                    message = $"Vé của SV {studentInfoForResponse.HoTen} (MSSV: {studentInfoForResponse.MSSV}), xe {ticket.BienSoXe}, đã hết hạn vào ngày {ticket.NgayHetHan:dd/MM/yyyy}.",
                    student = studentInfoForResponse,
                    ticketDetails = new { ticket.BienSoXe, NgayHetHan = ticket.NgayHetHan.ToString("dd/MM/yyyy") },
                    action = "expired"
                });
            }

            // Logic Check-in / Check-out 
            if (ticket.ParkingSlotId == null)
            {
                var availableParkingZone = await _context.ParkingSlots
                    .FirstOrDefaultAsync(pz => pz.CurrentCount < pz.MaxCapacity);

                if (availableParkingZone == null)
                {
                    _logger.LogInformation("ProcessTicketInternal: No available parking zones for check-in. Ticket ID: {TicketId}", ticket.ID_Ticket);
                    return Json(new
                    {
                        success = false,
                        message = "Hết chỗ! Tất cả các khu vực gửi xe đã đầy.",
                        student = studentInfoForResponse,
                        ticketDetails = new { ticket.BienSoXe },
                        action = "no_zone_slot"
                    });
                }

                ticket.ParkingSlotId = availableParkingZone.ParkingSlotId;
                ticket.ThoiGianVao = DateTime.Now;
                ticket.ThoiGianRa = null;
                availableParkingZone.CurrentCount++;

                _context.Tickets.Update(ticket);
                _context.ParkingSlots.Update(availableParkingZone);

                try
                {
                    await _context.SaveChangesAsync();
                    
                    // Create history record for check-in
                    var historyRecord = new History
                    {
                        TrangThai = "check-in",
                        BienSo = ticket.BienSoXe,
                        MSSV = studentInfoForResponse.MSSV,
                        TenSinhVien = studentInfoForResponse.HoTen,
                        Lop = studentInfoForResponse.Lop,
                        ThoiGian = ticket.ThoiGianVao.Value,
                        KhuVuc = availableParkingZone.SlotName,
                        TicketId = ticket.ID_Ticket
                    };
                    _context.Histories.Add(historyRecord);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Check-in successful: TicketID {TicketId}, Student {StudentMSSV}, Zone {ZoneName}, TimeIn {TimeIn}",
                                         ticket.ID_Ticket, studentInfoForResponse.MSSV, availableParkingZone.SlotName, ticket.ThoiGianVao);
                    return Json(new
                    {
                        success = true,
                        action = "checkin",
                        message = $"Check-in thành công cho SV {studentInfoForResponse.HoTen} (MSSV: {studentInfoForResponse.MSSV}), xe {ticket.BienSoXe} vào khu vực {availableParkingZone.SlotName}.",
                        slotName = availableParkingZone.SlotName,
                        student = studentInfoForResponse,
                        ticketDetails = new
                        {
                            ticket.BienSoXe,
                            NgayHetHan = ticket.NgayHetHan.ToString("dd/MM/yyyy"),
                            // SỬA Ở ĐÂY: Kiểm tra null cho ThoiGianVao trước khi gọi ToString
                            ThoiGianVao = ticket.ThoiGianVao.HasValue ? ticket.ThoiGianVao.Value.ToString("HH:mm dd/MM/yyyy") : null
                        }
                    });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "ProcessTicketInternal (Check-in): DbUpdateException for ticket ID {TicketId}", ticket.ID_Ticket);
                    availableParkingZone.CurrentCount--; // Rollback tạm thời nếu có lỗi
                    return StatusCode(500, new { success = false, message = "Lỗi cơ sở dữ liệu khi check-in. Vui lòng thử lại." });
                }
            }
            else // Xe đã ở trong bãi => Check-out
            {
                var currentParkingZone = ticket.ParkingSlot;
                if (currentParkingZone == null)
                {
                    _logger.LogError("ProcessTicketInternal (Check-out): Ticket ID {TicketId} has ParkingSlotId but ParkingSlot entity is null. ParkingSlotId: {ParkingSlotId}", ticket.ID_Ticket, ticket.ParkingSlotId);
                    return StatusCode(500, new { success = false, message = "Lỗi dữ liệu: Không tìm thấy thông tin khu vực gửi xe của vé này." });
                }

                string currentZoneName = currentParkingZone.SlotName;

                ticket.ThoiGianRa = DateTime.Now;
                currentParkingZone.CurrentCount--;
                if (currentParkingZone.CurrentCount < 0) currentParkingZone.CurrentCount = 0;

                int? previousSlotId = ticket.ParkingSlotId;
                ticket.ParkingSlotId = null;

                _context.Tickets.Update(ticket);
                _context.ParkingSlots.Update(currentParkingZone);

                try
                {
                    await _context.SaveChangesAsync();

                    // Create history record for check-out
                    var historyRecord = new History
                    {
                        TrangThai = "check-out",
                        BienSo = ticket.BienSoXe,
                        MSSV = studentInfoForResponse.MSSV,
                        TenSinhVien = studentInfoForResponse.HoTen,
                        Lop = studentInfoForResponse.Lop,
                        ThoiGian = ticket.ThoiGianRa.Value,
                        KhuVuc = currentZoneName,
                        TicketId = ticket.ID_Ticket
                    };
                    _context.Histories.Add(historyRecord);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Check-out successful: TicketID {TicketId}, Student {StudentMSSV}, FromZone {ZoneName}, TimeOut {TimeOut}",
                                         ticket.ID_Ticket, studentInfoForResponse.MSSV, currentZoneName, ticket.ThoiGianRa);
                    return Json(new
                    {
                        success = true,
                        action = "checkout",
                        message = $"Check-out thành công cho SV {studentInfoForResponse.HoTen} (MSSV: {studentInfoForResponse.MSSV}), xe {ticket.BienSoXe} từ khu vực {currentZoneName}.",
                        slotName = currentZoneName,
                        student = studentInfoForResponse,
                        ticketDetails = new
                        {
                            ticket.BienSoXe,
                            ThoiGianRa = ticket.ThoiGianRa.HasValue ? ticket.ThoiGianRa.Value.ToString("HH:mm dd/MM/yyyy") : null,
                            ThoiGianVao = ticket.ThoiGianVao.HasValue ? ticket.ThoiGianVao.Value.ToString("HH:mm dd/MM/yyyy") : null
                        }
                    });
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "ProcessTicketInternal (Check-out): DbUpdateException for ticket ID {TicketId}", ticket.ID_Ticket);
                    // Rollback tạm thời nếu có lỗi
                    currentParkingZone.CurrentCount++;
                    ticket.ParkingSlotId = previousSlotId;
                    return StatusCode(500, new { success = false, message = "Lỗi cơ sở dữ liệu khi check-out. Vui lòng thử lại." });
                }
            }
        }
    }

    // DTO cho request quét QR
    public class QRCodeScanRequest
    {
        public string? TicketIdContent { get; set; }
    }

    // DTO cho request nhập MSSV thủ công
    public class ManualCheckinRequest
    {
        public string? MSSV { get; set; }
    }
}