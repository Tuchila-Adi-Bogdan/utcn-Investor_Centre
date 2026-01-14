using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestorCenter.Data;
using InvestorCenter.Models;
using InvestorCenter.Areas.Identity.Data; // Ensure this matches where your User class is

namespace InvestorCenter.Controllers
{
    [Authorize] // 1. Restrict this entire controller to logged-in users
    public class PortfolioController : Controller
    {
        private readonly InvestorCenterContext _context;
        private readonly UserManager<InvestorCenterUser> _userManager;

        // 2. Inject dependencies
        public PortfolioController(InvestorCenterContext context, UserManager<InvestorCenterUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");

            // 3. Fetch the user's portfolio items AND include the Stock details
            var portfolioItems = await _context.PortfolioItems
                .Include(p => p.Stock)
                .Where(p => p.UserId == user.Id)
                .ToListAsync();

            // 4. Get the IDs of stocks the user owns to fetch chart data
            var stockIds = portfolioItems.Select(p => p.StockId).ToList();

            // 5. Fetch History (Last 10 points for the sparkline charts)
            // We only fetch history for the stocks the user actually owns
            var recentHistoryRaw = await _context.PriceHistories
                .Where(p => stockIds.Contains(p.StockId) && p.Timestamp > DateTime.UtcNow.AddHours(-1))
                .OrderByDescending(p => p.Timestamp)
                .ToListAsync();

            var recentHistory = recentHistoryRaw
                .GroupBy(p => p.StockId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Take(10).OrderBy(p => p.Timestamp).ToList()
                );

            // 6. Calculate Percentage Changes
            var percentageChanges = new Dictionary<int, decimal>();
            foreach (var stockGroup in recentHistory)
            {
                var historyPoints = stockGroup.Value;
                if (historyPoints.Count >= 2)
                {
                    var lastPrice = historyPoints[^1].Price;
                    var prevPrice = historyPoints[^2].Price;
                    if (prevPrice != 0)
                        percentageChanges[stockGroup.Key] = ((lastPrice - prevPrice) / prevPrice) * 100;
                }
            }

            // 7. Pass data to the View via ViewBag
            ViewBag.RecentHistory = recentHistory;
            ViewBag.PercentageChanges = percentageChanges;

            return View(portfolioItems);
        }
    }
}