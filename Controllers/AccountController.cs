using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Models;
using PBL3.Models.ViewModel;
namespace PBL3.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDBContext _context;

        public AccountController(UserManager<AppUser> userManager,
                               SignInManager<AppUser> signInManager,
                               AppDBContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault();

                if (role == "Student" || role == "Staff")
                {
                    return RedirectToAction("Profile", "Account");
                }
                else if (role == "Admin")
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Invalid Login Attempt.");
            return View(model);
        }
        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found!");
                return View(model);
            }
            else
            {
                return RedirectToAction("ChangePassword", "Account", new { username = user.UserName });
            }
        }

        [HttpGet]
        public IActionResult ChangePassword(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("VerifyEmail", "Account");
            }

            return View(new ChangePasswordViewModel { Email = username });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found!");
                return View(model);
            }

            var result = await _userManager.RemovePasswordAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                return RedirectToAction("Login", "Account");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Student,Staff")]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Lấy role của user
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            if (role == "Student")
            {
                // Lấy thông tin Student và Ticket liên quan
                var student = await _context.Users.OfType<Student>()
                    .Include(s => s.Tickets) // Include Tickets để lấy thông tin vé
                    .ThenInclude(t => t.ParkingSlot) // Include cả ParkingSlot để lấy thông tin vị trí
                    .FirstOrDefaultAsync(s => s.Id == user.Id);
                if (student == null)
                {
                    return NotFound();
                }

                // Lấy Ticket mới nhất của Student (nếu có)
                var latestTicket = student.Tickets?.OrderByDescending(t => t.NgayDangKy).FirstOrDefault();

                var model = new ProfileViewModel
                {
                    HoTen = student.HoTen,
                    Email = student.Email,
                    SDT = student.PhoneNumber,
                    Role = role,
                    MSSV = student.MSSV,
                    Lop = student.Lop,
                    // Thông tin Ticket (nếu có)
                    BienSoXe = latestTicket?.BienSoXe ?? "Không hợp lệ",
                    ViTriGui = latestTicket?.ParkingSlot?.SlotName ?? "Không hợp lệ",
                    NgayDangKy = latestTicket?.NgayDangKy ?? DateTime.MinValue,
                    NgayHetHan = latestTicket?.NgayHetHan ?? DateTime.MinValue,
                    Price = latestTicket?.Price ?? 0
                };
                return View(model);
            }
            else if (role == "Staff")
            {
                var staff = await _context.Users.OfType<Staff>().FirstOrDefaultAsync(s => s.Id == user.Id);
                if (staff == null)
                {
                    return NotFound();
                }

                var model = new ProfileViewModel
                {
                    HoTen = staff.HoTen,
                    Email = staff.Email,
                    SDT = staff.PhoneNumber,
                    Role = role,
                    DiaChi = staff.DiaChi
                };
                return View(model);
            }

            return NotFound();
        }
        [Authorize(Roles = "Student,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();
            model.Role = role ?? "";

            if (!ModelState.IsValid)
            {
                if (role == "Student")
                {
                    var studentWithTickets = await _context.Users.OfType<Student>()
                        .Include(s => s.Tickets)
                        .ThenInclude(t => t.ParkingSlot)
                        .FirstOrDefaultAsync(s => s.Id == user.Id);

                    var latestTicket = studentWithTickets?.Tickets?.OrderByDescending(t => t.NgayDangKy).FirstOrDefault();
                    model.BienSoXe = latestTicket?.BienSoXe ?? "Không hợp lệ";
                    model.ViTriGui = latestTicket?.ParkingSlot?.SlotName ?? "Không hợp lệ";
                    model.NgayDangKy = latestTicket?.NgayDangKy ?? DateTime.MinValue;
                    model.NgayHetHan = latestTicket?.NgayHetHan ?? DateTime.MinValue;
                    model.Price = latestTicket?.Price ?? 0;
                }
                return View(model);
            }

            if (role == "Student")
            {
                try
                {
                    var student = await _context.Users.OfType<Student>()
                        .Include(s => s.Tickets)
                        .ThenInclude(t => t.ParkingSlot)
                        .FirstOrDefaultAsync(s => s.Id == user.Id);

                    if (student == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin sinh viên
                    student.HoTen = model.HoTen;
                    student.PhoneNumber = model.SDT ?? "";
                    student.MSSV = model.MSSV;
                    student.Lop = model.Lop;

                    // Đánh dấu entity là đã được sửa đổi
                    _context.Entry(student).State = EntityState.Modified;

                    // Lưu thay đổi vào database
                    var saveResult = await _context.SaveChangesAsync();

                    if (saveResult > 0)
                    {
                        // Reload student from database to ensure we have latest data
                        await _context.Entry(student).ReloadAsync();

                        var latestTicket = student.Tickets?.OrderByDescending(t => t.NgayDangKy).FirstOrDefault();

                        var updatedModel = new ProfileViewModel
                        {
                            HoTen = student.HoTen,
                            Email = student.Email ?? "",
                            SDT = student.PhoneNumber ?? "",
                            Role = role,
                            MSSV = student.MSSV,
                            Lop = student.Lop,
                            BienSoXe = latestTicket?.BienSoXe ?? "Không hợp lệ",
                            ViTriGui = latestTicket?.ParkingSlot?.SlotName ?? "Không hợp lệ",
                            NgayDangKy = latestTicket?.NgayDangKy ?? DateTime.MinValue,
                            NgayHetHan = latestTicket?.NgayHetHan ?? DateTime.MinValue,
                            Price = latestTicket?.Price ?? 0
                        };

                        TempData["Success"] = "Cập nhật thông tin thành công.";
                        return View(updatedModel);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không thể cập nhật thông tin. Vui lòng thử lại.");
                        return View(model);
                    }
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật cơ sở dữ liệu: " + ex.Message);
                    return View(model);
                }
            }
            else if (role == "Staff")
            {
                try
                {
                    var staff = await _context.Users.OfType<Staff>()
                        .FirstOrDefaultAsync(s => s.Id == user.Id);

                    if (staff == null)
                    {
                        return NotFound();
                    }

                    staff.HoTen = model.HoTen;
                    staff.PhoneNumber = model.SDT ?? "";
                    staff.DiaChi = model.DiaChi ?? "";

                    // Đánh dấu entity là đã được sửa đổi
                    _context.Entry(staff).State = EntityState.Modified;

                    // Lưu thay đổi vào database
                    var saveResult = await _context.SaveChangesAsync();

                    if (saveResult > 0)
                    {
                        // Reload staff from database
                        await _context.Entry(staff).ReloadAsync();

                        var updatedModel = new ProfileViewModel
                        {
                            HoTen = staff.HoTen,
                            Email = staff.Email ?? "",
                            SDT = staff.PhoneNumber ?? "",
                            Role = role,
                            DiaChi = staff.DiaChi
                        };

                        TempData["Success"] = "Cập nhật thông tin thành công.";
                        return View(updatedModel);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không thể cập nhật thông tin. Vui lòng thử lại.");
                        return View(model);
                    }
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật cơ sở dữ liệu: " + ex.Message);
                    return View(model);
                }
            }

            return View(model);
        }
    }
}
