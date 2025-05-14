using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3.Data;
using PBL3.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace PBL3.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
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
        public IActionResult GetStats(string statType, int days)
        {
            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-days);

            var tickets = _context.Tickets
                .Where(t => t.NgayDangKy >= startDate && t.NgayDangKy <= endDate)
                .ToList();

            var (labels, revenueData, ticketCountData) = days switch
            {
                30 => GetDailyStats(tickets, startDate, endDate),
                _ => GetMonthlyStats(tickets, startDate, endDate)
            };

            return Json(new { labels, revenueData, ticketCountData });
        }

        private (List<string>, List<decimal>, List<int>) GetDailyStats(List<Ticket> tickets, DateTime startDate, DateTime endDate)
        {
            var dailyStats = tickets
                .GroupBy(t => t.NgayDangKy.Date)
                .ToDictionary(
                    g => g.Key,
                    g => (Revenue: g.Sum(t => t.Price), Count: g.Count())
                );

            var labels = new List<string>();
            var revenueData = new List<decimal>();
            var ticketCountData = new List<int>();

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                labels.Add(date.ToString("dd/MM/yyyy"));
                if (dailyStats.TryGetValue(date, out var stats))
                {
                    revenueData.Add(stats.Revenue);
                    ticketCountData.Add(stats.Count);
                }
                else
                {
                    revenueData.Add(0);
                    ticketCountData.Add(0);
                }
            }

            return (labels, revenueData, ticketCountData);
        }

        private (List<string>, List<decimal>, List<int>) GetMonthlyStats(List<Ticket> tickets, DateTime startDate, DateTime endDate)
        {
            var monthlyStats = tickets
                .GroupBy(t => new { t.NgayDangKy.Year, t.NgayDangKy.Month })
                .ToDictionary(
                    g => new DateTime(g.Key.Year, g.Key.Month, 1),
                    g => (Revenue: g.Sum(t => t.Price), Count: g.Count())
                );

            var labels = new List<string>();
            var revenueData = new List<decimal>();
            var ticketCountData = new List<int>();

            var currentDate = new DateTime(startDate.Year, startDate.Month, 1);
            var lastDate = new DateTime(endDate.Year, endDate.Month, 1);

            while (currentDate <= lastDate)
            {
                // Format as dd/MM/yyyy to match daily format and allow splitting
                labels.Add("01/" + currentDate.ToString("MM/yyyy"));
                if (monthlyStats.TryGetValue(currentDate, out var stats))
                {
                    revenueData.Add(stats.Revenue);
                    ticketCountData.Add(stats.Count);
                }
                else
                {
                    revenueData.Add(0);
                    ticketCountData.Add(0);
                }
                currentDate = currentDate.AddMonths(1);
            }

            return (labels, revenueData, ticketCountData);
        }
    }
}
