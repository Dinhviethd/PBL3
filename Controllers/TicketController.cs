using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PBL3.Data;
using PBL3.Models.ViewModel;
using PBL3.Models;
using Microsoft.EntityFrameworkCore;
[Authorize(Roles = "Student")]
public class TicketsController : Controller
{
    private readonly AppDBContext _context;
    private readonly UserManager<AppUser> _userManager;

    public TicketsController(AppDBContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // Hiển thị trang đăng ký vé
    public async Task<IActionResult> BuyTicket()
    {
        var user = await _userManager.GetUserAsync(User);
        var student = await _context.Users.OfType<Student>().FirstOrDefaultAsync(u => u.Id == user.Id);

        var model = new TicketViewModel
        {
            HoTen = user.HoTen,
            MSSV = student.MSSV,
            Lop = student.Lop
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicket(TicketViewModel model)
    {
        if (!ModelState.IsValid) return View("BuyTicket", model);

        var user = await _userManager.GetUserAsync(User);

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

        // 2. Xử lý tạo/cập nhật vé
        if (existingTicket != null && existingTicket.NgayHetHan > today)
        {
            // 2a. Nếu có vé chưa hết hạn: GIA HẠN
            existingTicket.NgayHetHan = existingTicket.NgayHetHan.AddMonths(monthsToAdd);
            existingTicket.Price += model.Price;
            _context.Update(existingTicket);
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
                ParkingSlotId = null // Để trống
            };
            _context.Tickets.Add(newTicket);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }
}