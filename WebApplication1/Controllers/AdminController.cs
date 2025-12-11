using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InvestorCenter.Services;

namespace InvestorCenter.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly StockUpdateSettings _settings;
        public AdminController(StockUpdateSettings settings)
        {
            _settings = settings;
        }

        public IActionResult Settings()
        {
            return View(_settings);
        }

        [HttpPost]
        public IActionResult UpdateSettings(StockUpdateSettings model)
        {
            _settings.UpdateDelayMs = model.UpdateDelayMs;
            return RedirectToAction("Settings");
        }
    }
}
