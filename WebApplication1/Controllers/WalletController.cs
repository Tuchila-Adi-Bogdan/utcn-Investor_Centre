using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using InvestorCenter.Areas.Identity.Data;

[Authorize]
public class WalletController : Controller
{
    private readonly UserManager<InvestorCenterUser> _userManager;

    // Dependency Injection
    public WalletController(UserManager<InvestorCenterUser> userManager)
    {
        _userManager = userManager;
    }


    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        ViewBag.Balance = user.Balance;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddMoney(decimal amount)
    {
        if (amount > 0)
        {
            var user = await _userManager.GetUserAsync(User);
            user.Balance += amount;
            await _userManager.UpdateAsync(user);
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> WithdrawMoney(decimal amount)
    {
        var user = await _userManager.GetUserAsync(User);

        if (amount <= 0)
        {
            TempData["ErrorMessage"] = "Amount must be greater than zero.";
        }
        else if (amount > user.Balance)
        {

            TempData["ErrorMessage"] = "Insufficient funds.";
        }
        else
        {
            user.Balance -= amount;
            await _userManager.UpdateAsync(user);
        }

        return RedirectToAction("Index");
    }
}