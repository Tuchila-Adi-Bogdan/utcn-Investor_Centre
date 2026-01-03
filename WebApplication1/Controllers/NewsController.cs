using InvestorCenter.Data;
using InvestorCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace InvestorCenter.Controllers
{
    public class NewsController : Controller
    {
        private readonly InvestorCenterContext _context;

        public NewsController(InvestorCenterContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var articles = _context.NewsArticles.Include(a => a.Effects).ToList();
            return View(articles);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddNews()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddNews(NewsArticle newArticle)
        {
            if (ModelState.IsValid)
            {
                newArticle.PublishedDate = DateTime.UtcNow;
                _context.NewsArticles.Add(newArticle);
                _context.SaveChanges();
                parseForEffects(newArticle);
                return RedirectToAction("Index");
            }
            return View(newArticle);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult ManageNews()
        {
            var articles = _context.NewsArticles.OrderByDescending(a => a.PublishedDate).ToList();
            return View(articles);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var article = _context.NewsArticles.Find(id);
            if (article != null)
            {
                // Optional: Also delete associated StockEffects if you want to clean up completely
                var effects = _context.StockEffects.Where(e => e.NewsArticleId == id);
                _context.StockEffects.RemoveRange(effects);

                _context.NewsArticles.Remove(article);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageNews");
        }
        public void parseForEffects(NewsArticle article)
        {
            if (article == null) return;

            // Load stocks into memory for efficient matching
            var stocks = _context.Stocks.ToList();
            if (!stocks.Any()) return;

            var text = $"{article.Title} {article.Content}";

            // Extract candidate tokens (simple heuristic: alphanumeric, ., - up to length 6)
            var tokenMatches = Regex.Matches(text, @"\b[A-Za-z0-9\.\-]{1,6}\b");
            var tokens = tokenMatches.Select(m => m.Value.Trim()).Where(t => !string.IsNullOrWhiteSpace(t))
                                     .Select(t => t.ToUpperInvariant()).Distinct().ToList();

            // Prepare fast lookup by ticker
            var tickerLookup = stocks.Where(s => !string.IsNullOrWhiteSpace(s.Ticker))
                                     .ToDictionary(s => s.Ticker.ToUpperInvariant(), s => s);

            var foundTickers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Match tokens against tickers
            foreach (var tok in tokens)
            {
                if (tickerLookup.TryGetValue(tok, out var stock))
                {
                    foundTickers.Add(stock.Ticker);
                }
            }

            // Also check company names (case-insensitive substring match)
            foreach (var stock in stocks)
            {
                if (string.IsNullOrWhiteSpace(stock.CompanyName)) continue;
                if (text.IndexOf(stock.CompanyName, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    foundTickers.Add(stock.Ticker);
                }
            }

            // Create StockEffect records for each found ticker if not already present for this article
            var existingEffectsForArticle = _context.StockEffects
                                                    .Where(e => e.NewsArticleId == article.Id)
                                                    .Select(e => e.Ticker)
                                                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

            Random random = new Random();

            var newEffects = foundTickers
                .Where(t => !existingEffectsForArticle.Contains(t))
                .Select(t => new StockEffect
                {
                    NewsArticleId = article.Id,
                    Ticker = t,
                    PriceChange = ((decimal)random.NextDouble() * 4) - 10,

                    ExpirationDate = DateTime.UtcNow.AddMinutes(5)
    }).ToList();

            if (newEffects.Any())
            {
                _context.StockEffects.AddRange(newEffects);
                _context.SaveChanges();
            }
        }

    }
}
