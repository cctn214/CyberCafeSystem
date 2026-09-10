using CCDatabase.Models;
using Microsoft.AspNetCore.Mvc;

namespace CCChartMVC.Controllers
{
    public class ChartJsController : Controller
    {
        private readonly AppDbContext _context;

        public ChartJsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var units = _context.Units.ToList();
            var rents = _context.Rents.ToList();

            var labels = new List<string>();
            var revenues = new List<decimal>();
            var rentCounts = new List<int>();
            var durations = new List<int>();

            foreach (var unit in units)
            {
                var unitRents = rents.Where(r => r.UnitId == unit.UnitId).ToList();
                labels.Add(unit.Type);
                revenues.Add(unitRents.Sum(r => r.TotalCost ?? 0));
                rentCounts.Add(unitRents.Count);
                durations.Add(unitRents.Sum(r => r.Duration ?? 0));
            }

            ViewBag.Labels = labels;
            ViewBag.Revenues = revenues;
            ViewBag.RentCounts = rentCounts;
            ViewBag.Durations = durations;
            ViewBag.TotalRevenue = revenues.Sum();
            ViewBag.TotalRents = rentCounts.Sum();
            ViewBag.TotalMinutes = durations.Sum();

            return View();
        }
    }
}
