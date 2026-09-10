using CCDatabase.Models;
using Microsoft.AspNetCore.Mvc;

namespace CCChartMVC.Controllers
{
    public class ApexChartsController : Controller
    {
        private readonly AppDbContext _context;

        public ApexChartsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var units = _context.Units.ToList();
            var rents = _context.Rents.ToList();

            // Chart 1: Sessions per Unit
            var labels = new List<string>();
            var series = new List<int>();

            foreach (var unit in units)
            {
                labels.Add(unit.Type);
                series.Add(rents.Count(r => r.UnitId == unit.UnitId));
            }

            // Chart 2: Category Revenue Breakdown (PC vs PS4 vs PS5)
            var categoryGroups = units
                .GroupBy(u => u.Type.StartsWith("PC") ? "PC Stations" : (u.Type.StartsWith("PS4") ? "PS4 Consoles" : "PS5 Consoles"))
                .Select(g => new
                {
                    Category = g.Key,
                    Revenue = rents.Where(r => g.Select(u => u.UnitId).Contains(r.UnitId)).Sum(r => r.TotalCost ?? 0)
                }).ToList();

            ViewBag.Labels = labels;
            ViewBag.Series = series;
            ViewBag.TotalSessions = series.Sum();

            ViewBag.CategoryLabels = categoryGroups.Select(c => c.Category).ToList();
            ViewBag.CategoryRevenues = categoryGroups.Select(c => c.Revenue).ToList();

            return View();
        }
    }
}
