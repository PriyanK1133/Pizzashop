using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pizzashop.Service.Utils;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Service.Interfaces;
using Pizzashop.Entity.Constants;
using Pizzashop.Web.Utils;
namespace Pizzashop.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IUserService _userService;
    private readonly IAccountService _accountService;
    private readonly ILocationService _locationService;

    public HomeController(IUserService userService, ILocationService locationService, IAccountService accountService)
    {
        _userService = userService;
        _locationService = locationService;
        _accountService = accountService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Profile()
    {
        var userId = SessionUtils.GetUserId(HttpContext);

        var response = await _userService.GetByIdAsync((Guid)userId!);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Index", "Dashboard");
        }

        var userVM = response.Data;

        return View(userVM);
    }

    public async Task<JsonResult> CountryBind()
    {
        var countries = await _locationService.GetAllCountriesAsync();
        SelectList countrylist = new(countries, "Id", "Name");
        return Json(countrylist);
    }

    public async Task<JsonResult> CityBind(Guid stateId)
    {
        var cities = await _locationService.GetCitiesByStateIdAsync(stateId);
        SelectList citylist = new(cities, "Id", "Name");
        return Json(citylist);

    }
    public async Task<JsonResult> StateBind(Guid countryId)
    {
        var states = await _locationService.GetStatesByCountryIdAsync(countryId);
        SelectList statelist = new(states, "Id", "Name");
        return Json(statelist);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(UserVM model)
    {
        ModelState.Remove("Email");
        ModelState.Remove("Password");
        ModelState.Remove("Role");
        if (!ModelState.IsValid)
        {
            TempData["error"] = "data is invalid! try again!!";
            return RedirectToAction("Profile");
        }
        var userId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        model.Id = userId;

        var response = await _userService.UpdateAsync(model);

        if (response.Success)
        {
            TempData["success"] = response.Message;
        }
        else
        {
            TempData["error"] = response.Message;
        }

        return RedirectToAction("Profile");
    }

    public async Task<IActionResult> UploadProfileImage(IFormFile? imageFile, string? profileImage)
    {
        if (imageFile == null)
        {
            FileUploadHandler.DeleteFile(profileImage);
            profileImage = null;
        }
        else
        {
            string? newProfileImage = await FileUploadHandler.UploadImage(imageFile);
            if (!string.IsNullOrEmpty(newProfileImage))
            {
                FileUploadHandler.DeleteFile(profileImage);
                profileImage = newProfileImage;
            }
        }

        Guid userId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        Response<bool> response = await _userService.UpdateProfileImageAsync(userId, profileImage);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
            RedirectToAction("Profile");
        }

        UserVM user = SessionUtils.GetUser(HttpContext)!;
        user.ProfileImage = profileImage ?? string.Empty;
        SessionUtils.SetUser(HttpContext, user);

        TempData["success"] = response.Message;
        return RedirectToAction("Profile");
    }

    public IActionResult ChangePassword()
    {
        bool isFirstLogin = SessionUtils.GetIsFirstLogin(HttpContext);

        if (isFirstLogin)
        {
            TempData["error"] = "Change your default password to access other functionality!";
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        UserVM user = SessionUtils.GetUser(HttpContext)!;
        model.Email = user.Email;

        var response = await _accountService.ChangePasswordAsync(model);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return View(model);
        }

        TempData["success"] = "Password Updated Successfully!";

        CookieUtils.ClearCookies(HttpContext);
        SessionUtils.ClearSession(HttpContext);

        if (user?.Role == Constants.Chef)
        {
            return RedirectToAction("KOT", "OrderApp");
        }
        return RedirectToAction("Index", "Dashboard");
    }


    public ActionResult Error()
    {
        if (Response.StatusCode == 403)
        {
            return View("Forbidden");
        }
        return View("NotFound"); // Or whatever your view is named
    }

}
