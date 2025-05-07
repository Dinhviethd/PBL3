using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PBL3.Data;
using PBL3.Models;
using PBL3.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace PBL3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StaffsController : Controller
    {
        private readonly AppDBContext _context;
        private readonly UserManager<AppUser> _userManager;

        public StaffsController(AppDBContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Staffs
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var appDBContext = _context.Staffs.Include(s => s.User);
            return View(await appDBContext.ToListAsync());
        }

        // GET: Staffs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // GET: Staffs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Staffs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StaffCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create AppUser
                var user = new AppUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    HoTen = model.HoTen,
                    PhoneNumber = model.PhoneNumber,
                    Role = "Staff",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Staff");

                    // Create Staff
                    var staff = new Staff
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = user.Id,
                        DiaChi = model.DiaChi
                    };

                    _context.Add(staff);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // GET: Staffs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staff == null)
            {
                return NotFound();
            }

            var viewModel = new StaffEditViewModel
            {
                Id = staff.Id,
                HoTen = staff.User.HoTen,
                Email = staff.User.Email,
                PhoneNumber = staff.User.PhoneNumber,
                DiaChi = staff.DiaChi
            };

            return View(viewModel);
        }

        // POST: Staffs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, StaffEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var staff = await _context.Staffs
                        .Include(s => s.User)
                        .FirstOrDefaultAsync(s => s.Id == id);

                    if (staff == null)
                    {
                        return NotFound();
                    }

                    // Update AppUser
                    staff.User.HoTen = model.HoTen;
                    staff.User.Email = model.Email;
                    staff.User.UserName = model.Email;
                    staff.User.PhoneNumber = model.PhoneNumber;

                    // Update Staff
                    staff.DiaChi = model.DiaChi;

                    await _userManager.UpdateAsync(staff.User);
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Staffs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = await _context.Staffs
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // POST: Staffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var staff = await _context.Staffs
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
            
            if (staff != null)
            {
                _context.Staffs.Remove(staff);
                await _userManager.DeleteAsync(staff.User);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StaffExists(string id)
        {
            return _context.Staffs.Any(e => e.Id == id);
        }
    }
}
