using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class NewsController : Controller
    {
        private readonly WebApplication1Context _context;

        public NewsController(WebApplication1Context context)
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
                return RedirectToAction("Index");
            }
            return View(newArticle);
        }

    }
}
