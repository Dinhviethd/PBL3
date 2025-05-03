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



        [HttpGet]
        [Authorize(Roles = "Admin")] 
        public IActionResult Register()
        {
            ViewBag.Roles = new List<string> { "Staff", "Student" };
            return View("QLSV");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStudent(RegisterStudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("QLSV", await GetStudentViewModel(model));
            }

            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                HoTen = model.HoTen,
                Role = "Student",
                PhoneNumber = model.SDT, 
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Student");

                var student = new Student
                {
                    Id = user.Id, 
                    HoTen = model.HoTen,
                    Email = model.Email,
                    MSSV = model.MSSV,
                    Lop = model.Lop,
                    DKyVe = false,
                    PhoneNumber = model.SDT 
                };

                _context.Student.Add(student);
                await _context.SaveChangesAsync();

                return RedirectToAction("QLSV");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("QLSV", await GetStudentViewModel(model));
        }

        private async Task<RegisterStudentViewModel> GetStudentViewModel(RegisterStudentViewModel model = null, int page = 1,
    string searchName = "", string searchClass = "", string searchMSSV = "", string searchPhone = "")
        {
            int pageSize = 10;

            var query = _context.Student.AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(s => s.HoTen.Contains(searchName));

            if (!string.IsNullOrEmpty(searchClass))
                query = query.Where(s => s.Lop.Contains(searchClass));

            if (!string.IsNullOrEmpty(searchMSSV))
                query = query.Where(s => s.MSSV.Contains(searchMSSV));

            if (!string.IsNullOrEmpty(searchPhone))
                query = query.Where(s => s.PhoneNumber.Contains(searchPhone));

            var totalItems = await query.CountAsync();
            var students = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            var viewModel = model ?? new RegisterStudentViewModel();
            viewModel.Students = students;
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

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> QLSV(int page = 1, string searchName = "", string searchClass = "", string searchMSSV = "", string searchPhone = "")
        {
            var model = await GetStudentViewModel(null, page, searchName, searchClass, searchMSSV, searchPhone);
            return View(model);
        }
       
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditStudent(int id)
        {
            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var model = new RegisterStudentViewModel
            {
                HoTen = student.HoTen,
                Email = student.Email,
                MSSV = student.MSSV,
                Lop = student.Lop,
                Role = "Student"
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(int id, RegisterStudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            student.HoTen = model.HoTen;
            student.Email = model.Email;
            student.MSSV = model.MSSV;
            student.Lop = model.Lop;

            _context.Student.Update(student);
            await _context.SaveChangesAsync();

            return RedirectToAction("QLSV");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            // Xóa user tương ứng nếu cần
            var user = await _userManager.FindByEmailAsync(student.Email);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            _context.Student.Remove(student);
            await _context.SaveChangesAsync();

            return RedirectToAction("QLSV");
        }

    }
    }
