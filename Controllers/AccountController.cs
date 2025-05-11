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
                return RedirectToAction("Index", "Home");
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
            model.Role = role;

            if (!ModelState.IsValid)
            {
                return View(model);
            }



            if (role == "Student")
            {
                var student = await _context.Users.OfType<Student>()
                    .Include(s => s.Tickets)
                        .ThenInclude(t => t.ParkingSlot) // Include cả ParkingSlot để lấy thông tin vị trí
                    .FirstOrDefaultAsync(s => s.Id == user.Id);
                if (student == null)
                {
                    return NotFound();
                }

                // Chỉ cập nhật các thông tin cơ bản, không cập nhật thông tin Ticket
                student.HoTen = model.HoTen;
                student.PhoneNumber = model.SDT;
                student.MSSV = model.MSSV;
                student.Lop = model.Lop;

                var result = await _userManager.UpdateAsync(student);
                if (result.Succeeded)
                {
                    // Lấy Ticket mới nhất của Student (nếu có)
                    var latestTicket = student.Tickets?.OrderByDescending(t => t.NgayDangKy).FirstOrDefault();

                    // Cập nhật lại toàn bộ model với thông tin mới
                    var updatedModel = new ProfileViewModel
                    {
                        HoTen = student.HoTen,
                        Email = student.Email,
                        SDT = student.PhoneNumber,
                        Role = role, // Đảm bảo role luôn là Student
                        MSSV = student.MSSV,
                        Lop = student.Lop,
                        // Thông tin Ticket (nếu có)
                        BienSoXe = latestTicket?.BienSoXe ?? "Không hợp lệ",
                        ViTriGui = latestTicket?.ParkingSlot?.SlotName ?? "Không hợp lệ",
                        NgayDangKy = latestTicket?.NgayDangKy ?? DateTime.MinValue,
                        NgayHetHan = latestTicket?.NgayHetHan ?? DateTime.MinValue,
                        Price = latestTicket?.Price ?? 0
                    };

                    TempData["Success"] = "Cập nhật thông tin thành công.";
                    return View(updatedModel);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else if (role == "Staff")
            {
                var staff = await _context.Users.OfType<Staff>().FirstOrDefaultAsync(s => s.Id == user.Id);
                if (staff == null)
                {
                    return NotFound();
                }

                staff.HoTen = model.HoTen;
                staff.PhoneNumber = model.SDT;
                staff.DiaChi = model.DiaChi;

                var result = await _userManager.UpdateAsync(staff);
                if (result.Succeeded)
                {
                    // Cập nhật lại toàn bộ model với thông tin mới
                    var updatedModel = new ProfileViewModel
                    {
                        HoTen = staff.HoTen,
                        Email = staff.Email,
                        SDT = staff.PhoneNumber,
                        Role = role, // Đảm bảo role luôn là Staff
                        DiaChi = staff.DiaChi
                    };

                    TempData["Success"] = "Cập nhật thông tin thành công.";
                    return View(updatedModel);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}
