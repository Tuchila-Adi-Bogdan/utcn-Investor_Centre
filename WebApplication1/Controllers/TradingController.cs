using InvestorCenter.Areas.Identity.Data;
using InvestorCenter.Data;
using InvestorCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvestorCenter.Controllers
{
    public class TradingController : Controller
    {
        private readonly InvestorCenterContext _context;
        private readonly UserManager<InvestorCenterUser> _userManager;

        public TradingController(InvestorCenterContext context, UserManager<InvestorCenterUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        [Authorize(Roles = "Admin")]
        public IActionResult ManageStocks()
        {
            var stocks = _context.Stocks.ToList();
            return View(stocks);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult EditStockPage(int id)
        {
            var stock = _context.Stocks.Find(id);
            if (stock != null)
                return View(stock);
            else
                return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, Stock stock)
        {
            if (id != stock.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _context.Stocks.Update(stock);
                _context.SaveChanges();
                return RedirectToAction("ManageStocks");
            }
            return View(stock);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var stock = _context.Stocks.Find(id);
            if (stock != null)
            {
                _context.Stocks.Remove(stock);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageStocks");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Buy(int stockId, int quantity)
        {
            if (quantity <= 0) return RedirectToAction("Index");

            var user = await _userManager.GetUserAsync(User);
            var stock = await _context.Stocks.FindAsync(stockId);

            if (stock == null) return NotFound();

            decimal totalCost = stock.Price * quantity;

            // 1. Check if user has enough money
            if (user.Balance < totalCost)
            {
                TempData["Error"] = "Insufficient funds!";
                return RedirectToAction("Details", "Trading", new { id = stockId });
            }

            // 2. Deduct money
            user.Balance -= totalCost;

            // 3. Add stock to portfolio
            var portfolioItem = _context.PortfolioItems
                .FirstOrDefault(p => p.UserId == user.Id && p.StockId == stockId);

            if (portfolioItem == null)
            {
                // First time buying this stock
                portfolioItem = new PortfolioItem
                {
                    UserId = user.Id,
                    StockId = stockId,
                    Quantity = quantity,
                    AveragePrice = stock.Price
                };
                _context.PortfolioItems.Add(portfolioItem);
            }
            else
            {
                // Already owns it, update quantity and average price
                // New Avg = ((OldQty * OldAvg) + (NewQty * NewPrice)) / TotalQty
                decimal currentValue = portfolioItem.Quantity * portfolioItem.AveragePrice;
                decimal newValue = quantity * stock.Price;
                portfolioItem.AveragePrice = (currentValue + newValue) / (portfolioItem.Quantity + quantity);
                portfolioItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Successfully bought {quantity} shares of {stock.Ticker}";

            return RedirectToAction("Index", "Portfolio");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Sell(int stockId, int quantity)
        {
            if (quantity <= 0) return RedirectToAction("Index");

            var user = await _userManager.GetUserAsync(User);
            var stock = await _context.Stocks.FindAsync(stockId);

            // 1. Check if user owns the stock
            var portfolioItem = _context.PortfolioItems
                .FirstOrDefault(p => p.UserId == user.Id && p.StockId == stockId);

            if (portfolioItem == null || portfolioItem.Quantity < quantity)
            {
                TempData["Error"] = "You don't own enough shares to sell.";
                return RedirectToAction("Details", "Trading", new { id = stockId });
            }

            // 2. Calculate revenue
            decimal revenue = stock.Price * quantity;

            // 3. Add money to user
            user.Balance += revenue;

            // 4. Remove stock from portfolio
            portfolioItem.Quantity -= quantity;

            if (portfolioItem.Quantity == 0)
            {
                _context.PortfolioItems.Remove(portfolioItem);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Successfully sold {quantity} shares of {stock.Ticker}";

            return RedirectToAction("Index", "Portfolio");
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
