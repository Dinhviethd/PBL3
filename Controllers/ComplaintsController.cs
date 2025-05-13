using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Models;
using PBL3.Models.ViewModel;
using System.Security.Claims;

namespace PBL3.Controllers
{
    public class ComplaintsController : Controller
    {
        private readonly AppDBContext _context;

        public ComplaintsController(AppDBContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Submit([FromBody] ComplaintSubmitModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.Content))
            {
                return Json(new { success = false });
            }

            try
            {
                var complaint = new Complaint
                {
                    Title = model.Title,
                    Content = model.Content,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    CreatedAt = DateTime.Now,
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
