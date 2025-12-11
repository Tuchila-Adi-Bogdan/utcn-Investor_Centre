using Microsoft.AspNetCore.Mvc;

namespace InvestorCenter.Controllers
{
    public class PortfolioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
