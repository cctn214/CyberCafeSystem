using CCDatabase.Models;
using Microsoft.AspNetCore.Mvc;

namespace CCChartMVC.Controllers
{
    public class CanvasJsController : Controller
    {
        private readonly AppDbContext _context;

        public CanvasJsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            var rents = _context.Rents.ToList();

            var spendingPoints = new List<object>();
            var balancePoints = new List<object>();
            var sessionPoints = new List<object>();

            foreach (var user in users)
            {
                var userRents = rents.Where(r => r.UserId == user.UserId).ToList();
                var totalSpent = userRents.Sum(r => r.TotalCost ?? 0);

                spendingPoints.Add(new { label = user.Name, y = (double)totalSpent });
                balancePoints.Add(new { label = user.Name, y = (double)user.Balance });
                sessionPoints.Add(new { label = user.Name, y = userRents.Count });
            }

            ViewBag.SpendingPoints = spendingPoints;
            ViewBag.BalancePoints = balancePoints;
            ViewBag.SessionPoints = sessionPoints;

            return View();
        }
    }
}
