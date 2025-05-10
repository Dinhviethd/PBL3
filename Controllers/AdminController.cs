using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Models.ViewModel;
using PBL3.Models;
using PBL3.Data;

namespace PBL3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDBContext _context;

        public AdminController(UserManager<AppUser> userManager, AppDBContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Quản lý sinh viên (QLSV)
        public async Task<IActionResult> QLSV(int page = 1, string searchType = "MSSV", string searchValue = "")
        {
            ViewBag.SearchType = searchType;
            ViewBag.SearchValue = searchValue;

            var query = _context.Users.OfType<Student>().AsQueryable();

            if (!string.IsNullOrEmpty(searchValue))
            {
                switch (searchType)
                {
                    case "Name":
                        query = query.Where(s => s.HoTen.Contains(searchValue));
                        break;
                    case "Class":
                        query = query.Where(s => s.Lop.Contains(searchValue));
                        break;
                    case "MSSV":
                        query = query.Where(s => s.MSSV.Contains(searchValue));
                        break;
                    case "Phone":
                        query = query.Where(s => s.PhoneNumber.Contains(searchValue));
                        break;
                }
            }

            var pageSize = 10;
            var totalItems = await query.CountAsync();
            var students = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new RegisterStudentViewModel
            {
                Students = students,
                PageInfo = new PageInfo
                {
                    CurrentPage = page,
                    TotalItems = totalItems,
                    ItemsPerPage = pageSize,
                    TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
                }
            };

            return View(model);
        }

        // GET: Đăng ký tài khoản
        public IActionResult Register()
        {
            return View();
        }

        // POST: Đăng ký tài khoản
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email đã tồn tại chưa
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Email này đã được sử dụng.");
                    return View(model);
                }

                // Tạo user tùy theo role
                AppUser user;
                if (model.Role == "Student")
                {
                    var studentModel = new StudentRegisterViewModel
                    {
                        HoTen = model.HoTen,
                        Email = model.Email,
                        SDT = model.SDT,
                        Password = model.Password,
                        ConfirmPassword = model.ConfirmPassword,
                        Role = model.Role,
                        MSSV = Request.Form["MSSV"].ToString(),
                        Lop = Request.Form["Lop"].ToString()
                    };

                    // Validate student specific fields
                    if (string.IsNullOrEmpty(studentModel.MSSV))
                    {
                        ModelState.AddModelError(string.Empty, "MSSV là bắt buộc cho sinh viên.");
                        return View(model);
                    }

                    if (string.IsNullOrEmpty(studentModel.Lop))
                    {
                        ModelState.AddModelError(string.Empty, "Lớp là bắt buộc cho sinh viên.");
                        return View(model);
                    }

                    // Kiểm tra MSSV đã tồn tại chưa
                    var existingStudent = await _context.Users.OfType<Student>()
                        .FirstOrDefaultAsync(s => s.MSSV == studentModel.MSSV);
                    if (existingStudent != null)
                    {
                        ModelState.AddModelError(string.Empty, "MSSV này đã được sử dụng.");
                        return View(model);
                    }

                    user = new Student
                    {
                        UserName = studentModel.Email,
                        Email = studentModel.Email,
                        HoTen = studentModel.HoTen,
                        PhoneNumber = studentModel.SDT,
                        MSSV = studentModel.MSSV,
                        Lop = studentModel.Lop
                    };
                }
                else // Staff
                {
                    var staffModel = new StaffRegisterViewModel
                    {
                        HoTen = model.HoTen,
                        Email = model.Email,
                        SDT = model.SDT,
                        Password = model.Password,
                        ConfirmPassword = model.ConfirmPassword,
                        Role = model.Role,
                        DiaChi = Request.Form["DiaChi"].ToString()
                    };

                    // Validate staff specific fields
                    if (string.IsNullOrEmpty(staffModel.DiaChi))
                    {
                        ModelState.AddModelError(string.Empty, "Địa chỉ là bắt buộc cho nhân viên.");
                        return View(model);
                    }

                    user = new Staff
                    {
                        UserName = staffModel.Email,
                        Email = staffModel.Email,
                        HoTen = staffModel.HoTen,
                        PhoneNumber = staffModel.SDT,
                        DiaChi = staffModel.DiaChi
                    };
                }

                // Tạo user với password
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Thêm role cho user
                    await _userManager.AddToRoleAsync(user, model.Role);
                    return RedirectToAction("QLSV");
                }

                // Nếu có lỗi, thêm vào ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: Sửa thông tin sinh viên
        public async Task<IActionResult> EditStudent(string id)
        {
            var student = await _context.Users.OfType<Student>().FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            var model = new RegisterStudentViewModel
            {
                HoTen = student.HoTen,
                Email = student.Email,
                SDT = student.PhoneNumber,
                MSSV = student.MSSV,
                Lop = student.Lop,
                Role = "Student"
            };

            return View(model);
        }

        // POST: Sửa thông tin sinh viên
        [HttpPost]
        public async Task<IActionResult> EditStudent(string id, RegisterStudentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var student = await _context.Users.OfType<Student>().FirstOrDefaultAsync(s => s.Id == id);
                if (student == null)
                {
                    return NotFound();
                }

                // Kiểm tra MSSV mới có bị trùng không (nếu có thay đổi)
                if (student.MSSV != model.MSSV)
                {
                    var existingStudent = await _context.Users.OfType<Student>()
                        .FirstOrDefaultAsync(s => s.MSSV == model.MSSV);
                    if (existingStudent != null)
                    {
                        ModelState.AddModelError(string.Empty, "MSSV này đã được sử dụng.");
                        return View(model);
                    }
                }

                // Cập nhật thông tin student
                student.HoTen = model.HoTen;
                student.Email = model.Email;
                student.PhoneNumber = model.SDT;
                student.MSSV = model.MSSV;
                student.Lop = model.Lop;

                var result = await _userManager.UpdateAsync(student);
                if (result.Succeeded)
                {
                    return RedirectToAction("QLSV");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // GET: Xác nhận xóa sinh viên
        public async Task<IActionResult> ConfirmDeleteStudent(string id)
        {
            var student = await _context.Users.OfType<Student>().FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            var hasTickets = await _context.Set<Ticket>().AnyAsync(t => t.StudentId == id);
            if (hasTickets)
            {
                ViewBag.HasTickets = true;
                ViewBag.Message = "Sinh viên đã có vé giữ xe, bạn chắc chắn muốn xóa chứ?";
            }
            else
            {
                ViewBag.HasTickets = false;
                ViewBag.Message = "Bạn chắc chắn muốn xóa sinh viên này?";
            }

            return View(student);
        }

        // POST: Xóa sinh viên
        [HttpPost]
        public async Task<IActionResult> DeleteStudent(string id)
        {
            var student = await _context.Users.OfType<Student>().FirstOrDefaultAsync(s => s.Id == id);
            if (student != null)
            {
                // Xóa tất cả ticket của sinh viên
                var tickets = await _context.Tickets.Where(t => t.StudentId == id).ToListAsync();
                _context.Tickets.RemoveRange(tickets);

                // Xóa sinh viên
                var result = await _userManager.DeleteAsync(student);
                if (result.Succeeded)
                {
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Đã xóa sinh viên thành công.";
                }
                else
                {
                    TempData["Error"] = "Có lỗi xảy ra khi xóa sinh viên.";
                }
            }
            return RedirectToAction("QLSV");
        }
    }
}