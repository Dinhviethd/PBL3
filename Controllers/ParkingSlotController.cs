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

        public ParkingSlotController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, string searchType)
        {
            var viewModel = new ParkingSlotViewModel
            {
                SearchTerm = searchTerm,
                SearchType = searchType,
                ParkingSlots = await _context.ParkingSlots.ToListAsync()
            };

            var query = _context.Tickets
                .Include(t => t.Student)
                .Include(t => t.ParkingSlot)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                switch (searchType)
                {
                    case "mssv":
                        query = query.Where(t => t.Student.MSSV.Contains(searchTerm));
                        break;
                    case "name":
                        query = query.Where(t => t.Student.HoTen.Contains(searchTerm));
                        break;
                    case "license":
                        query = query.Where(t => t.BienSoXe.Contains(searchTerm));
                        break;
                    case "entry-date":
                        query = query.Where(t => t.NgayDangKy.ToString().Contains(searchTerm));
                        break;
                    case "exit-date":
                        query = query.Where(t => t.NgayHetHan.ToString().Contains(searchTerm));
                        break;
                    case "not-entered":
                        query = query.Where(t => t.ParkingSlotId == null);
                        break;
                }
            }

            viewModel.Tickets = await query.ToListAsync();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTickets(int[] ticketIds)
        {
            if (ticketIds != null && ticketIds.Length > 0)
            {
                var tickets = await _context.Tickets
                    .Where(t => ticketIds.Contains(t.ID_Ticket))
                    .ToListAsync();

                _context.Tickets.RemoveRange(tickets);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
