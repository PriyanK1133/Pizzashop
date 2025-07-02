using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Interfaces;
using Pizzashop.Entity.Constants;
using Pizzashop.Web.Utils;

namespace Pizzashop.Web.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            UserVM? user = SessionUtils.GetUser(HttpContext);
            if (user?.Role == Constants.Chef)
            {
                return RedirectToAction("KOT", "OrderApp");
            }
            return RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(LoginVM model)
    {
        if (ModelState.IsValid)
        {
            var response = await _accountService.AuthenticateUserAsync(model);
            if (!response.Success)
            {
                TempData["error"] = response.Message;
                return View(model);
            }
            TempData["success"] = response.Message;

            // Store token in cookie
            var loginResponse = response.Data!;

            CookieUtils.SaveAccessToken(HttpContext.Response, loginResponse.AccessToken);
            if (loginResponse.RefreshToken != null)
            {
                CookieUtils.SaveRefreshToken(HttpContext.Response, loginResponse.RefreshToken);
            }

            return RedirectToAction("Index");
        }
        return View(model);
    }

    public IActionResult ForgotPassword(string email)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            UserVM? user = SessionUtils.GetUser(HttpContext);
            if (user?.Role == Constants.Chef)
            {
                return RedirectToAction("KOT", "OrderApp");
            }
            return RedirectToAction("Index", "Dashboard");
        }
        LoginVM loginVM = new()
        {
            Email = email
        };
        return View(loginVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ForgotPassword(LoginVM model)
    {
        ModelState.Remove("Password");
        if (!ModelState.IsValid)
        {
            return View();
        }
        string baseUrl = Request.Scheme + "://" + Request.Host.Host + (Request.Host.Port.HasValue ? $":{Request.Host.Port.Value}" : "");
        var response = await _accountService.ForgotPasswordAsync(model, baseUrl);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return View(model);
        }

        TempData["success"] = response.Message;

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> ResetPassword(Guid id, Guid token)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            UserVM? user = SessionUtils.GetUser(HttpContext);
            if (user?.Role == Constants.Chef)
            {
                return RedirectToAction("KOT", "OrderApp");
            }
            return RedirectToAction("Index", "Dashboard");
        }

        var isTokenValid = await _accountService.IsTokenValidAsync(id, token);
        if (!isTokenValid)
        {
            TempData["error"] = "Invalid reset password link";
            return RedirectToAction("Index");
        }

        ResetPasswordVM resetPasswordVM = new()
        {
            Id = id,
            Token = token
        };

        return View(resetPasswordVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var response = await _accountService.ResetPasswordAsync(model);

        if (response.Success)
        {
            TempData["success"] = response.Message;
        }
        else
        {
            TempData["error"] = response.Message;
        }

        return RedirectToAction("Index");
    }

    public IActionResult Logout()
    {
        CookieUtils.ClearCookies(HttpContext);
        SessionUtils.ClearSession(HttpContext);
        return RedirectToAction("Index");
    }
}
