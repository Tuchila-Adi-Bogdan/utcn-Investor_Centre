using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestorCenter.Data;
using InvestorCenter.Models;

namespace InvestorCenter.Controllers
{
    public class TradingController : Controller
    {
        private readonly InvestorCenterContext _context;

        public TradingController(InvestorCenterContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Details(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null) return NotFound();

            var sinceDate = DateTime.UtcNow.AddMinutes(-1);
            var history = await _context.PriceHistories
                .Where(p => p.StockId == id && p.Timestamp >= sinceDate)
                .OrderBy(p => p.Timestamp)
                .ToListAsync();

            ViewData["Stock"] = stock;
            return View(history);
        }

        public async Task<IActionResult> Index()
        {
            var stocks = await _context.Stocks.ToListAsync();

            // 1. Fetch recent raw data from the database (e.g., last hour)
            var recentHistoryRaw = await _context.PriceHistories
                .Where(p => p.Timestamp > DateTime.UtcNow.AddHours(-1))
                .OrderByDescending(p => p.Timestamp)
                .ToListAsync();

            // 2. Group and process the data in your application's memory
            var recentHistory = recentHistoryRaw
                .GroupBy(p => p.StockId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Take(10).OrderBy(p => p.Timestamp).ToList() // Take the 10 most recent points
                );

            var percentageChanges = new Dictionary<int, decimal>();
            foreach (var stockGroup in recentHistory)
            {
                var historyPoints = stockGroup.Value;
                if (historyPoints.Count >= 2)
                {
                    var lastPrice = historyPoints[^1].Price; // C# 8 index from end
                    var previousPrice = historyPoints[^2].Price;
                    if (previousPrice != 0)
                    {
                        var change = ((lastPrice - previousPrice) / previousPrice) * 100;
                        percentageChanges[stockGroup.Key] = change;
                    }
                }
            }

            ViewBag.RecentHistory = recentHistory;
            ViewBag.PercentageChanges = percentageChanges;

            return View(stocks);
        }

        [HttpGet]
        public async Task<IActionResult> GetHistoryData(int stockId, string range)
        {
            var sinceDate = DateTime.UtcNow;

            switch (range.ToLower())
            {
                case "1min": sinceDate = sinceDate.AddMinutes(-1); break;
                case "5min": sinceDate = sinceDate.AddMinutes(-5); break;
                case "10min": sinceDate = sinceDate.AddMinutes(-10); break;
                case "1hour": sinceDate = sinceDate.AddHours(-1); break;
                case "1day": sinceDate = sinceDate.AddDays(-1); break;
                case "1week": sinceDate = sinceDate.AddDays(-7); break;
                case "1month": sinceDate = sinceDate.AddMonths(-1); break;
                default: sinceDate = sinceDate.AddMinutes(-1); break;
            }

            var history = await _context.PriceHistories
                .Where(p => p.StockId == stockId && p.Timestamp >= sinceDate)
                .OrderBy(p => p.Timestamp)
                .Select(p => new { timestamp = p.Timestamp, price = p.Price })
                .ToListAsync();

            return Json(history);
        }


        [Authorize(Roles = "Admin")]
        public IActionResult AddStock()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddStock(Stock newStock)
        {
            if (ModelState.IsValid)
            {
                _context.Stocks.Add(newStock);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(newStock);
        }

    }
}
