using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Areas.Identity.Data;

[Authorize] // Ensures only logged-in users can access this
public class WalletController : Controller
{
    private readonly UserManager<InvestorCenterUser> _userManager;

    // Inject UserManager to get the current user
    public WalletController(UserManager<InvestorCenterUser> userManager)
    {
        _userManager = userManager;
    }

    // GET: /Wallet/Index
    public async Task<IActionResult> Index()
    {
        // Get the currently logged-in user
        var user = await _userManager.GetUserAsync(User);

        // Pass the user's balance to the view
        ViewBag.Balance = user.Balance;

        return View();
    }

    // POST: /Wallet/AddMoney
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

    // POST: /Wallet/WithdrawMoney
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
            // Use TempData to show an error message on the view
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