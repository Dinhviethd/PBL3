using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Models;
using System.Linq;

namespace PBL3.Controllers
{
    public class StatController : Controller
    {
        private readonly AppDBContext _context;

        public StatController(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View("~/Views/Shared/Stat.cshtml");
        }

        [HttpGet]
        public async Task<IActionResult> GetMonthlyStats(int? month, int? year)
        {
            // If month or year not provided, use current date
            var today = DateTime.Now;
            month ??= today.Month;
            year ??= today.Year;

            // Get first and last day of the month
            var firstDayOfMonth = new DateTime(year.Value, month.Value, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Get all tickets for the month
            var tickets = await _context.Tickets
                .Where(t => t.NgayDangKy >= firstDayOfMonth && t.NgayDangKy <= lastDayOfMonth)
                .ToListAsync();

            // Group by date and calculate daily stats
            var dailyStats = tickets
                .GroupBy(t => t.NgayDangKy.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(t => t.Price),
                    TicketCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            // Calculate monthly totals
            var monthlyStats = new
            {
                TotalRevenue = dailyStats.Sum(x => x.Revenue),
                TotalTickets = dailyStats.Sum(x => x.TicketCount),
                DailyData = new
                {
                    Labels = dailyStats.Select(x => x.Date.ToString("dd/MM")).ToArray(),
                    RevenueData = dailyStats.Select(x => (double)x.Revenue).ToArray(),
                    TicketCountData = dailyStats.Select(x => x.TicketCount).ToArray()
                }
            };

            return Json(monthlyStats);
        }

        [HttpGet]
        public async Task<IActionResult> GetStats(string statType, int days)
        {
            // Calculate the start date based on the selected period
            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-days);

            // Get tickets within the date range
            var tickets = await _context.Tickets
                .Where(t => t.NgayDangKy >= startDate && t.NgayDangKy <= endDate)
                .ToListAsync();

            // Group by date and calculate daily stats
            var dailyStats = tickets
                .GroupBy(t => t.NgayDangKy.Date)
                .OrderBy(g => g.Key)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(t => t.Price),
                    TicketCount = g.Count()
                })
                .ToList();

            // Create list of all dates in range (to include days with no tickets)
            var allDates = Enumerable.Range(0, days)
                .Select(offset => endDate.AddDays(-offset).Date)
                .OrderBy(date => date);

            // Fill in missing dates with zero values
            var completeStats = allDates.GroupJoin(
                dailyStats,
                date => date,
                stat => stat.Date,
                (date, stats) => new
                {
                    Date = date,
                    Revenue = stats.FirstOrDefault()?.Revenue ?? 0,
                    TicketCount = stats.FirstOrDefault()?.TicketCount ?? 0
                })
                .OrderBy(x => x.Date)
                .ToList();

            // Prepare the response data
            var response = new
            {
                labels = completeStats.Select(x => x.Date.ToString("dd/MM/yyyy")).ToArray(),
                revenueData = completeStats.Select(x => (double)x.Revenue).ToArray(),
                ticketCountData = completeStats.Select(x => x.TicketCount).ToArray()
            };

            return Json(response);
        }
    }
}
