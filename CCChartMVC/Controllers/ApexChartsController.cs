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

            var labels = new List<string>();
            var series = new List<int>();

            foreach (var unit in units)
            {
                labels.Add(unit.Type);
                series.Add(rents.Count(r => r.UnitId == unit.UnitId));
            }

            ViewBag.Labels = labels;
            ViewBag.Series = series;
            ViewBag.TotalSessions = series.Sum();

            return View();
        }
    }
}
