using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestorCenter.Data;
using InvestorCenter.Models;
using InvestorCenter.Areas.Identity.Data;

namespace InvestorCenter.Controllers
{
    [Authorize]
    public class PortfolioController : Controller
    {
        private readonly InvestorCenterContext _context;
        private readonly UserManager<InvestorCenterUser> _userManager;

        // Dependency Injection
        public PortfolioController(InvestorCenterContext context, UserManager<InvestorCenterUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Home");

            var portfolioItems = await _context.PortfolioItems
                .Include(p => p.Stock)
                .Where(p => p.UserId == user.Id)
                .ToListAsync();

            var stockIds = portfolioItems.Select(p => p.StockId).ToList();

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

            ViewBag.RecentHistory = recentHistory;
            ViewBag.PercentageChanges = percentageChanges;

            return View(portfolioItems);
        }
    }
}