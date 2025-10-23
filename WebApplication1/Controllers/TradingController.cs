using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class TradingController : Controller
    {
        public IActionResult Index()
        {
            var stocks = new List<Stock>
            { 
                new Stock { Id = 1, Symbol = "AAPL", CompanyName = "Apple Inc.", Price = 150.25M },
                new Stock { Id = 2, Symbol = "MSFT", CompanyName = "Microsoft Corporation", Price = 295.50M },
                new Stock { Id = 3, Symbol = "GOOGL", CompanyName = "Alphabet Inc.", Price = 2800.75M }
            };
            return View(stocks);
        }
    }
}
