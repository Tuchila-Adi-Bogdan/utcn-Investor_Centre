using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApplication1.Data;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly WebApplication1Context _context;

    public HomeController(ILogger<HomeController> logger, WebApplication1Context context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = new HomeViewModel();
        viewModel.Stocks = await _context.Stocks.ToListAsync();
        viewModel.NewsArticles = await _context.NewsArticles
            .OrderByDescending(n => n.PublishedDate)
            .Take(3)
            .ToListAsync();

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

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }
    public IActionResult Trading()
    {
        return View();
    }
    public IActionResult News()
    {
        return View();
    }
    public IActionResult Register()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
