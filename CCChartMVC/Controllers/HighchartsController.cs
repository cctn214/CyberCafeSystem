using CCDatabase.Models;
using Microsoft.AspNetCore.Mvc;

namespace CCChartMVC.Controllers
{
    public class HighchartsController : Controller
    {
        private readonly AppDbContext _context;

        public HighchartsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var units = _context.Units.ToList();
            var rents = _context.Rents.ToList();

            var totalRevenue = rents.Sum(r => r.TotalCost ?? 0);
            var pieData = new List<object>();
            var columnCategories = new List<string>();
            var columnRevenues = new List<double>();

            foreach (var unit in units)
            {
                var unitRevenue = rents.Where(r => r.UnitId == unit.UnitId).Sum(r => r.TotalCost ?? 0);
                double percentage = totalRevenue > 0 ? Math.Round((double)(unitRevenue / totalRevenue) * 100, 1) : 0;

                pieData.Add(new { name = unit.Type, y = percentage });
                columnCategories.Add(unit.Type);
                columnRevenues.Add((double)unitRevenue);
            }

            ViewBag.PieData = pieData;
            ViewBag.ColumnCategories = columnCategories;
            ViewBag.ColumnRevenues = columnRevenues;

            return View();
        }
    }
}
