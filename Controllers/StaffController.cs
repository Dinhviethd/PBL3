using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Models;
using PBL3.Models.ViewModel;

namespace PBL3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StaffController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDBContext _context;

        public StaffController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            AppDBContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, string searchTerm = "")
        {
            var viewModel = await GetStaffViewModel(null, page, searchTerm);
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StaffViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                HoTen = model.HoTen,
                Role = "Staff",
                PhoneNumber = model.SDT,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Staff");

                var staff = new Staff
                {
                    Id = user.Id,
                    HoTen = model.HoTen,
                    Email = model.Email,
                    DiaChi = model.DiaChi,
                    PhoneNumber = model.SDT
                };

                _context.Staff.Add(staff);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            var viewModel = new StaffViewModel
            {
                HoTen = staff.HoTen,
                Email = staff.Email,
                SDT = staff.PhoneNumber,
                DiaChi = staff.DiaChi
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, StaffViewModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            staff.HoTen = model.HoTen;
            staff.PhoneNumber = model.SDT;
            staff.DiaChi = model.DiaChi;

            try
            {
                _context.Update(staff);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StaffExists(staff.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffExists(string id)
        {
            return _context.Staff.Any(e => e.Id == id);
        }

        private async Task<StaffViewModel> GetStaffViewModel(StaffViewModel model = null, int page = 1, string searchTerm = "")
        {
            int pageSize = 10;

            var query = _context.Staff.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(s => 
                    s.HoTen.Contains(searchTerm) || 
                    s.Email.Contains(searchTerm) ||
                    s.PhoneNumber.Contains(searchTerm)
                );
            }

            var totalItems = await query.CountAsync();
            var staffs = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var viewModel = model ?? new StaffViewModel();
            viewModel.Staffs = staffs;
            viewModel.PageInfo = new PageInfo
            {
                CurrentPage = page,
                TotalItems = totalItems,
                ItemsPerPage = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                StartRecord = (page - 1) * pageSize + 1,
                EndRecord = Math.Min(page * pageSize, totalItems)
            };

            return viewModel;
        }
    }
} 