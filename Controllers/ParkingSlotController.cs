using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Models;
using PBL3.Models.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace PBL3.Controllers
{
    public class ParkingSlotController : Controller
    {
        private readonly AppDBContext _context;
        private readonly ILogger<ParkingSlotController> _logger; // Thêm Logger

        public ParkingSlotController(AppDBContext context, ILogger<ParkingSlotController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string searchTerm, string searchType)
        {
            _logger.LogInformation("ParkingSlot Index: SearchTerm='{SearchTerm}', SearchType='{SearchType}'", searchTerm, searchType);
            var parkingSlots = await _context.ParkingSlots.OrderBy(s => s.SlotName).ToListAsync(); // Sắp xếp ô xe
            var viewModel = new ParkingSlotViewModel
            {
                SearchTerm = searchTerm,
                SearchType = searchType,
                ParkingSlots = parkingSlots
            };

            var query = _context.Tickets
                .Include(t => t.Student)
                .Include(t => t.ParkingSlot)
                .AsQueryable();

            if (searchType == "not-entered")
            {
                query = query.Where(t => t.ParkingSlotId == null && t.ThoiGianVao == null); // Chỉ xe chưa vào bãi và chưa check-in
            }
            else
            {

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    switch (searchType)
                    {
                        case "mssv":
                            query = query.Where(t => t.Student != null && (t.Student as Student).MSSV.Contains(searchTerm));
                            break;
                        case "name":
                            query = query.Where(t => t.Student != null && t.Student.HoTen.Contains(searchTerm));
                            break;
                        case "license":
                            query = query.Where(t => t.BienSoXe.Contains(searchTerm));
                            break;
                    }
                }
            }

            viewModel.Tickets = await query.OrderByDescending(t => t.ID_Ticket).ToListAsync();
            return View(viewModel);
        }



        [HttpPost]
        public async Task<IActionResult> DeleteTickets(int[] ticketIds)
        {
            if (ticketIds != null && ticketIds.Length > 0)
            {
                var ticketsToUpdate = await _context.Tickets
                    .Include(t => t.ParkingSlot) // Cần để cập nhật CurrentCount
                    .Where(t => ticketIds.Contains(t.ID_Ticket))
                    .ToListAsync();

                foreach (var ticket in ticketsToUpdate)
                {
                    if (ticket.ParkingSlotId != null && ticket.ThoiGianVao != null && ticket.ThoiGianRa == null)
                    {
                        // Nếu vé đang ở trong bãi (đã check-in, chưa check-out)
                        // Thì khi "xóa" (unassign), phải giảm CurrentCount của ParkingSlot
                        var parkingSlot = ticket.ParkingSlot; // Đã include
                        if (parkingSlot != null)
                        {
                            parkingSlot.CurrentCount = Math.Max(0, parkingSlot.CurrentCount - 1);
                        }
                        ticket.ThoiGianRa = DateTime.Now; // Đánh dấu thời gian ra khi bị xóa/unassign khỏi ô
                    }
                    ticket.ParkingSlotId = null; // Bỏ gán khỏi ô
                }
                await _context.SaveChangesAsync();
                _logger.LogInformation("Unassigned/Marked as left {Count} tickets.", ticketsToUpdate.Count);
            }
            else
            {
                _logger.LogWarning("DeleteTickets POST: No ticketIds provided.");
            }
            // Redirect về trang Index với các tham số tìm kiếm hiện tại để giữ trạng thái filter
            return RedirectToAction(nameof(Index));
        }



        [HttpGet]
        public async Task<IActionResult> GetTicketsBySlot(int slotId)
        {
            var tickets = await _context.Tickets
                    .Include(t => t.Student)
                    .Where(t => t.ParkingSlotId == slotId && t.ThoiGianRa == null) // Chỉ lấy vé đang thực sự trong ô (chưa check-out)
                    .OrderByDescending(t => t.ThoiGianVao)
                    .ToListAsync();

            return Json(new
            {
                success = true,
                data = tickets.Select(t => new
                {
                    id = t.ID_Ticket,
                    studentName = t.Student?.HoTen ?? "N/A",
                    studentId = (t.Student as Student)?.MSSV ?? "N/A",
                    licensePlate = t.BienSoXe,
                    registrationDate = t.NgayDangKy.ToString("dd/MM/yyyy"),
                    checkInDate = t.ThoiGianVao?.ToString("dd/MM/yyyy HH:mm") ?? "Lỗi: Thiếu TGVao",
                    ticketExpiryDate = t.NgayHetHan.ToString("dd/MM/yyyy")
                })
            });
        }
    }
}

