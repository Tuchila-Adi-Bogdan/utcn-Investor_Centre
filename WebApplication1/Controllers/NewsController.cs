using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestorCenter.Data;
using InvestorCenter.Models;

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
                //newArticle.PublishedDate = DateTime.UtcNow;
                _context.NewsArticles.Add(newArticle);
                _context.SaveChanges();
                //parseForEffects(newArticle);
                return RedirectToAction("Index");
            }
            return View(newArticle);
        }

        public void parseForEffects(NewsArticle article)
        {
            StockEffect effect = new StockEffect();
            effect.NewsArticleId = article.Id;

            _context.StockEffects.Add(effect);
            _context.SaveChanges();

        }

    }
}
