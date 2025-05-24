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
                    var studentModel = new RegisterStudentViewModel
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
                        Lop = studentModel.Lop,
                        Tickets = new List<Ticket>()
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
                    if (model.Role == "Student") return RedirectToAction("QLSV");
                    else return RedirectToAction("QLNV");

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
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var student = await _context.Users.OfType<Student>().FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            var model = new EditStudentViewModel
            {
                Id = student.Id,
                HoTen = student.HoTen ?? string.Empty,
                Email = student.Email ?? string.Empty,
                SDT = student.PhoneNumber ?? string.Empty,
                MSSV = student.MSSV ?? string.Empty,
                Lop = student.Lop ?? string.Empty
            };

            return View(model);
        }

        // POST: Sửa thông tin sinh viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(string id, EditStudentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrEmpty(id) || id != model.Id)
            {
                return NotFound();
            }

            var student = await _context.Users.OfType<Student>().FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            // Kiểm tra email mới có bị trùng không (nếu có thay đổi)
            if (!string.Equals(student.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && !string.Equals(existingUser.Id, id, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty, "Email này đã được sử dụng bởi người dùng khác.");
                    return View(model);
                }
            }

            // Kiểm tra MSSV mới có bị trùng không (nếu có thay đổi)
            if (!string.Equals(student.MSSV, model.MSSV, StringComparison.OrdinalIgnoreCase))
            {
                var existingStudent = await _context.Users.OfType<Student>()
                    .FirstOrDefaultAsync(s => s.MSSV == model.MSSV && !string.Equals(s.Id, id, StringComparison.OrdinalIgnoreCase));
                if (existingStudent != null)
                {
                    ModelState.AddModelError(string.Empty, "MSSV này đã được sử dụng bởi sinh viên khác.");
                    return View(model);
                }
            }

            try
            {
                // Cập nhật thông tin student
                student.HoTen = model.HoTen;
                student.Email = model.Email;
                student.UserName = model.Email;
                student.PhoneNumber = model.SDT;
                student.MSSV = model.MSSV;
                student.Lop = model.Lop;

                var result = await _userManager.UpdateAsync(student);
                if (result.Succeeded)
                {
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thông tin sinh viên thành công.";
                    return RedirectToAction("QLSV");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật thông tin: " + ex.Message);
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

        public async Task<IActionResult> QLNV(int page = 1, string searchType = "Email", string searchValue = "")
        {
            ViewBag.SearchType = searchType;
            ViewBag.SearchValue = searchValue;

            var query = _context.Users.OfType<Staff>().AsQueryable();

            if (!string.IsNullOrEmpty(searchValue))
            {
                switch (searchType)
                {
                    case "Name":
                        query = query.Where(s => s.HoTen.Contains(searchValue));
                        break;
                    case "Email":
                        query = query.Where(s => s.Email.Contains(searchValue));
                        break;
                    case "Phone":
                        query = query.Where(s => s.PhoneNumber.Contains(searchValue));
                        break;
                    case "Address":
                        query = query.Where(s => s.DiaChi.Contains(searchValue));
                        break;
                }
            }

            var pageSize = 10;
            var totalItems = await query.CountAsync();
            var staffs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new StaffRegisterViewModel
            {
                Staffs = staffs,
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

        // GET: Sửa thông tin nhân viên
        public async Task<IActionResult> EditStaff(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var staff = await _context.Users.OfType<Staff>().FirstOrDefaultAsync(s => s.Id == id);
            if (staff == null)
            {
                return NotFound();
            }

            var model = new EditStaffViewModel
            {
                Id = staff.Id,
                HoTen = staff.HoTen ?? string.Empty,
                Email = staff.Email ?? string.Empty,
                SDT = staff.PhoneNumber ?? string.Empty,
                DiaChi = staff.DiaChi ?? string.Empty
            };

            return View(model);
        }

        // POST: Sửa thông tin nhân viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStaff(string id, EditStaffViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrEmpty(id) || id != model.Id)
            {
                return NotFound();
            }

            var staff = await _context.Users.OfType<Staff>().FirstOrDefaultAsync(s => s.Id == id);
            if (staff == null)
            {
                return NotFound();
            }

            // Kiểm tra email mới có bị trùng không (nếu có thay đổi)
            if (!string.Equals(staff.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && !string.Equals(existingUser.Id, id, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError(string.Empty, "Email này đã được sử dụng bởi người dùng khác.");
                    return View(model);
                }
            }

            try
            {
                // Cập nhật thông tin staff
                staff.HoTen = model.HoTen;
                staff.Email = model.Email;
                staff.UserName = model.Email;
                staff.PhoneNumber = model.SDT;
                staff.DiaChi = model.DiaChi;

                var result = await _userManager.UpdateAsync(staff);
                if (result.Succeeded)
                {
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thông tin nhân viên thành công.";
                    return RedirectToAction("QLNV");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật thông tin: " + ex.Message);
            }
            
            return View(model);
        }

        // GET: Xác nhận xóa nhân viên
        public async Task<IActionResult> ConfirmDeleteStaff(string id)
        {
            var staff = await _context.Users.OfType<Staff>().FirstOrDefaultAsync(s => s.Id == id);
            if (staff == null)
            {
                return NotFound();
            }

            ViewBag.Message = "Bạn chắc chắn muốn xóa nhân viên này?";
            return View(staff);
        }

        // POST: Xóa nhân viên
        [HttpPost]
        public async Task<IActionResult> DeleteStaff(string id)
        {
            var staff = await _context.Users.OfType<Staff>().FirstOrDefaultAsync(s => s.Id == id);
            if (staff != null)
            {
                var result = await _userManager.DeleteAsync(staff);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Đã xóa nhân viên thành công.";
                }
                else
                {
                    TempData["Error"] = "Có lỗi xảy ra khi xóa nhân viên.";
                }
            }
            return RedirectToAction("QLNV");
        }

        // GET: Reset password
        public async Task<IActionResult> ResetPassword(string id, string role)
        {
            AppUser user = null;
            if (role == "Staff")
            {
                user = await _context.Users.OfType<Staff>().FirstOrDefaultAsync(s => s.Id == id);
            }
            else if (role == "Student")
            {
                user = await _context.Users.OfType<Student>().FirstOrDefaultAsync(s => s.Id == id);
            }

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.UserName = user.HoTen;
            ViewBag.UserId = user.Id;
            ViewBag.UserType = role == "Staff" ? "nhân viên" : "sinh viên";
            ViewBag.Role = role;
            return View();
        }

        // Complaints management
        public async Task<IActionResult> Complaints()
        {
            var complaints = await _context.Complaints
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return View(complaints);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateComplaintStatus(int complaintId, string status)
        {
            var complaint = await _context.Complaints.FindAsync(complaintId);

            if (complaint == null)
            {
                return Json(new { success = false, message = "Không tìm thấy khiếu nại" });
            }

            try
            {
                complaint.Status = Enum.Parse<ComplaintStatus>(status);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false, message = "Cập nhật trạng thái thất bại" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComplaint(int id)
        {
            var complaint = await _context.Complaints.FindAsync(id);

            if (complaint == null)
            {
                return Json(new { success = false });
            }

            try
            {
                _context.Complaints.Remove(complaint);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetComplaintDetails(int id)
        {
            var complaint = await _context.Complaints
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.ComplaintId == id);

            if (complaint == null)
            {
                return Json(new { success = false });
            }

            return Json(new
            {
                success = true,
                title = complaint.Title,
                content = complaint.Content,
                userName = complaint.User.HoTen,
                createdAt = complaint.CreatedAt.ToString("dd-MM-yyyy THH:mm:ss"), // ISO format
                status = complaint.Status.ToString()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitComplaint([FromBody] ComplaintSubmitModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false });

            try
            {
                var user = await _userManager.GetUserAsync(User);
                var complaint = new Complaint
                {
                    UserId = user.Id,
                    Title = model.Title,
                    Content = model.Content,
                    CreatedAt = DateTime.UtcNow,
                    Status = ComplaintStatus.Pending
                };

                _context.Complaints.Add(complaint);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
    }
}