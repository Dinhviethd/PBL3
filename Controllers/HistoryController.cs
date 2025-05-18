using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PBL3.Controllers
{
    [Authorize(Roles = "Staff,Student")]
    public class HistoryController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<AppUser> _userManager;

        public HistoryController(AppDBContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> ViewHistory(string searchTerm = "", string searchType = "")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var query = _context.Histories
                .Include(h => h.Ticket)
                .AsQueryable();

            if (User.IsInRole("Student"))
            {
                var student = await _context.Users.OfType<Student>().FirstOrDefaultAsync(s => s.Id == user.Id);
                if (student != null)
                {
                    // If user is a student, only show their own history
                    query = query.Where(h => h.MSSV == student.MSSV);
                }
            }
            else if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Search functionality only for staff
                switch (searchType?.ToLower())
                {
                    case "bienso":
                        query = query.Where(h => h.BienSo.Contains(searchTerm));
                        break;
                    case "mssv":
                        query = query.Where(h => h.MSSV.Contains(searchTerm));
                        break;
                    case "tensinhvien":
                        query = query.Where(h => h.TenSinhVien.Contains(searchTerm));
                        break;
                    case "trangthai":
                        query = query.Where(h => h.TrangThai.Contains(searchTerm));
                        break;
                    case "lop":
                        query = query.Where(h => h.Lop.Contains(searchTerm));
                        break;
                }
            }

            var histories = await query.OrderByDescending(h => h.ThoiGian).ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SearchType = searchType;

            return View(histories);
        }
    }
}