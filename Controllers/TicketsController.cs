using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PBL3.Data;
using PBL3.Models.ViewModel;
using PBL3.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using QuestPDF.Fluent;
using QRCoder;

public class TicketsController : Controller
{
    private readonly AppDBContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<TicketsController> _logger;

    public TicketsController(AppDBContext context, UserManager<AppUser> userManager, ILogger<TicketsController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IActionResult> BuyTicket()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var student = await _context.Users.OfType<Student>().FirstOrDefaultAsync(u => u.Id == user.Id);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            var model = new TicketViewModel
            {
                HoTen = user.HoTen,
                MSSV = student.MSSV,
                Lop = student.Lop
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in BuyTicket action");
            return RedirectToAction("Error", "Home");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTicket([FromForm] TicketViewModel model)
    {
        _logger.LogInformation("CreateTicket action started with model: {@Model}", model);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Model state is invalid: {@ModelState}", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            return View("BuyTicket", model);
        }

        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("User not found");
                return RedirectToAction("Login", "Account");
            }

            // Debug user information
            _logger.LogInformation("User ID: {UserId}, Email: {Email}", user.Id, user.Email);

            // 1. Kiểm tra vé hiện tại của sinh viên
            var existingTicket = await _context.Tickets
                .Where(t => t.StudentId == user.Id && t.BienSoXe == model.BienSoXe)
                .OrderByDescending(t => t.NgayHetHan)
                .FirstOrDefaultAsync();

            var today = DateTime.Now;
            int monthsToAdd = model.PackageName switch
            {
                "Gói 1 Tháng" => 1,
                "Gói 3 Tháng" => 3,
                "Gói 6 Tháng" => 6,
                _ => 0
            };

            _logger.LogInformation("Processing ticket for user {UserId}, existing ticket: {@ExistingTicket}",
                user.Id, existingTicket);

            // 2. Xử lý tạo/cập nhật vé
            if (existingTicket != null && existingTicket.NgayHetHan > today)
            {
                // 2a. Nếu có vé chưa hết hạn: GIA HẠN
                existingTicket.NgayHetHan = existingTicket.NgayHetHan.AddMonths(monthsToAdd);
                existingTicket.Price += model.Price;
                _context.Update(existingTicket);
                _logger.LogInformation("Extending existing ticket: {@Ticket}", existingTicket);
            }
            else
            {
                // 2b. Nếu không có vé hoặc đã hết hạn: TẠO MỚI
                var newTicket = new Ticket
                {
                    BienSoXe = model.BienSoXe,
                    NgayDangKy = today,
                    NgayHetHan = today.AddMonths(monthsToAdd),
                    Price = model.Price,
                    StudentId = user.Id,
                    ParkingSlotId = null, // Để trống
                    ThoiGianVao = today,
                    ThoiGianRa = null
                };
                _context.Tickets.Add(newTicket);
                _logger.LogInformation("Creating new ticket: {@Ticket}", newTicket);
            }

            var result = await _context.SaveChangesAsync();
            _logger.LogInformation("SaveChanges result: {Result}", result);

            if (result > 0)
            {
                TempData["SuccessMessage"] = "Đăng ký vé thành công!";
                return RedirectToAction("BuyTicket", model);
            }
            else
            {
                ModelState.AddModelError("", "Không thể lưu thông tin vé. Vui lòng thử lại.");
                return View("BuyTicket", model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in CreateTicket action");
            ModelState.AddModelError("", "Đã xảy ra lỗi khi xử lý yêu cầu của bạn.");
            return View("BuyTicket", model);
        }

    }
    public async Task<IActionResult> PrintTicket(string searchLicensePlate = "", string searchStudentName = "")
    {
        var query = _context.Tickets
            .Include(t => t.ParkingSlot)
            .Include(t => t.Student)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchLicensePlate))
        {
            query = query.Where(t => t.BienSoXe.Contains(searchLicensePlate));
        }

        if (!string.IsNullOrWhiteSpace(searchStudentName))
        {
            query = query.Where(t => t.Student.HoTen.Contains(searchStudentName));
        }

        var tickets = await query.ToListAsync();

        ViewBag.SearchLicensePlate = searchLicensePlate;
        ViewBag.SearchStudentName = searchStudentName;

        return View(tickets);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckAndUpdateTicket([FromBody] TicketUpdateRequest request)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin người dùng." });
            }

            var existingTicket = await _context.Tickets
                .Where(t => t.StudentId == user.Id)
                .OrderByDescending(t => t.NgayHetHan)
                .FirstOrDefaultAsync();

            var today = DateTime.Now;
            var hasValidTicket = existingTicket != null && existingTicket.NgayHetHan > today;

            if (hasValidTicket)
            {
                // Calculate months to add based on package
                int monthsToAdd = request.PackageName switch
                {
                    "Gói 1 Tháng" => 1,
                    "Gói 3 Tháng" => 3,
                    "Gói 6 Tháng" => 6,
                    _ => 0
                };

                // Update existing ticket
                existingTicket.NgayHetHan = existingTicket.NgayHetHan.AddMonths(monthsToAdd);
                existingTicket.Price += request.Price;
                _context.Update(existingTicket);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    hasValidTicket = true,
                    licensePlate = existingTicket.BienSoXe,
                    newExpiryDate = existingTicket.NgayHetHan.ToString("dd/MM/yyyy")
                });
            }

            return Json(new
            {
                success = true,
                hasValidTicket = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking and updating ticket");
            return Json(new { success = false, message = "Có lỗi xảy ra khi xử lý yêu cầu." });
        }
    }

    public class TicketUpdateRequest
    {
        public string PackageName { get; set; }
        public decimal Price { get; set; }
    }
    [Authorize(Roles = "Staff")]
    public async Task<IActionResult> PrintTicketPdf(int id)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Student)
            .Include(t => t.ParkingSlot)
            .FirstOrDefaultAsync(t => t.ID_Ticket == id);

        if (ticket == null)
        {
            return NotFound();
        }

        var studentDetails = ticket.Student as Student; // Ép kiểu AppUser sang Student
        // 1. Tạo nội dung cho QR Code (ID_Ticket)
        string qrContent = ticket.ID_Ticket.ToString();
        _logger.LogInformation("PrintTicketPdf GET: QR content for ticket ID {TicketId} will be '{QRContent}'.", id, qrContent);

        // 2. Tạo QR Code Data
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q); // Mức sửa lỗi Q (khá tốt)

        // 3. Tạo ảnh QR Code dưới dạng byte array (PNG)
        PngByteQRCode pngQrCode = new PngByteQRCode(qrCodeData);
        byte[] qrCodeBytes = pngQrCode.GetGraphic(20); // 20 pixels per module

        // 4. Chuyển byte array thành chuỗi Base64
        string qrCodeBase64 = Convert.ToBase64String(qrCodeBytes);
        _logger.LogDebug("PrintTicketPdf GET: Generated Base64 QR code (length: {Length}) for ticket ID {TicketId}.", qrCodeBase64.Length, id);

        // Tạo model cho PDF
        var model = new TicketPdfModel
        {
            HoTen = ticket.Student.HoTen,
            MSSV = (ticket.Student as Student)?.MSSV,
            BienSoXe = ticket.BienSoXe,
            NgayDangKy = ticket.NgayDangKy,
            NgayHetHan = ticket.NgayHetHan,
            Price = ticket.Price,
            SlotName = ticket.ParkingSlot?.SlotName ?? "Chưa chỉ định",
            QRCode = qrCodeBase64
        };

        // Tạo PDF
        var document = new TicketPdfDocument(model);
        var stream = new MemoryStream();
        document.GeneratePdf(stream);
        stream.Position = 0;

        // Trả về file PDF
        return File(stream, "application/pdf", $"VeXe_{ticket.BienSoXe}_{DateTime.Now:yyyyMMdd}.pdf");
    }
}