using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class PortfolioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
