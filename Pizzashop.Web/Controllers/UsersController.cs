
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;
using Pizzashop.Entity.ViewModel;
using Pizzashop.Entity.Constants;
using Pizzashop.Web.Attributes;
using Pizzashop.Entity.Helper;
using Pizzashop.Web.Utils;

namespace Pizzashop.Web.Controllers;

[PermissionAuthorize(Constants.CanView)]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    private readonly ILocationService _locationService;
    private readonly IRoleAndPermissionService _roleAndPermissionService;
    private readonly IWebHostEnvironment _environment;

    public UsersController(IUserService userService, ILocationService locationService, IRoleAndPermissionService roleAndPermissionService, IWebHostEnvironment environment)
    {
        _userService = userService;
        _locationService = locationService;
        _roleAndPermissionService = roleAndPermissionService;
        _environment = environment;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> GetPagedUsers(string searchString = "", int page = 1, int pagesize = 5, string sortColumn = "", bool isASC = true)
    {

        Response<PagedResult<UserListVM>> response = await _userService.GetUsersAsync(searchString, page, pagesize, sortColumn, isASC);
        if (!response.Success)
        {
            return PartialView("_Users");
        }

        PagedResult<UserListVM> model = response.Data!;
        return PartialView("_Users", model);
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> Add()
    {
        string role = SessionUtils.GetUser(HttpContext)!.Role!;

        IEnumerable<CountryVM> countries = await _locationService.GetAllCountriesAsync();
        Response<IEnumerable<RoleVM>> roles = await _roleAndPermissionService.GetAllRolesAsync();

        ViewBag.Countries = new SelectList(countries, "Id", "Name");
        ViewBag.Roles = new SelectList(roles.Data, "Id", "Name").Where(r => RolePriority.GetRolePriority(r.Text) < RolePriority.GetRolePriority(role));

        return View();
    }

    [HttpPost]
    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> Add(UserVM model, IFormFile? imageFile)
    {
        string role = SessionUtils.GetUser(HttpContext)!.Role!;

        IEnumerable<CountryVM> countries = await _locationService.GetAllCountriesAsync();
        if (!model.CountryId.Equals(Guid.Empty))
        {
            IEnumerable<StateVM> states = await _locationService.GetStatesByCountryIdAsync(model.CountryId);
            ViewBag.States = new SelectList(states, "Id", "Name");
        }
        if (!model.StateId.Equals(Guid.Empty))
        {
            IEnumerable<CityVM> cities = await _locationService.GetCitiesByStateIdAsync(model.StateId);
            ViewBag.Cities = new SelectList(cities, "Id", "Name");
        }
        Response<IEnumerable<RoleVM>> roles = await _roleAndPermissionService.GetAllRolesAsync();

        ViewBag.Countries = new SelectList(countries, "Id", "Name");
        ViewBag.Roles = new SelectList(roles.Data, "Id", "Name").Where(r => RolePriority.GetRolePriority(r.Text) < RolePriority.GetRolePriority(role));
        ModelState.Remove("IsActive");
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        Guid creatorId = (Guid)SessionUtils.GetUserId(HttpContext)!;

        string profileImage = await FileUploadHandler.UploadImage(imageFile);
        if (!string.IsNullOrEmpty(profileImage))
        {
            FileUploadHandler.DeleteFile(model.ProfileImage);
            model.ProfileImage = profileImage;
        }

        Response<UserVM?> response = await _userService.AddAsync(model, creatorId);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return View(model);
        }

        TempData["success"] = response.Message;
        return RedirectToAction("Index");
    }

    [PermissionAuthorize(Constants.CanView)]
    public async Task<IActionResult> Edit(Guid id)
    {
        Guid currentUserId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        if (currentUserId == id)
        {
            return RedirectToAction("Profile", "Home");
        }

        string role = SessionUtils.GetUser(HttpContext)!.Role!;

        Response<UserVM?> response = await _userService.GetByIdAsync(id);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Index");
        }

        UserVM? userVM = response.Data;
        IEnumerable<CountryVM> countries = await _locationService.GetAllCountriesAsync();
        IEnumerable<StateVM> states = await _locationService.GetStatesByCountryIdAsync(userVM!.CountryId);
        IEnumerable<CityVM> cities = await _locationService.GetCitiesByStateIdAsync(userVM.StateId);
        Response<IEnumerable<RoleVM>> roles = await _roleAndPermissionService.GetAllRolesAsync();

        ViewBag.Countries = new SelectList(countries, "Id", "Name");
        ViewBag.States = new SelectList(states, "Id", "Name");
        ViewBag.Cities = new SelectList(cities, "Id", "Name");
        ViewBag.Roles = new SelectList(roles.Data, "Id", "Name").Where(r => RolePriority.GetRolePriority(r.Text) < RolePriority.GetRolePriority(role));

        return View(userVM);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<IActionResult> Edit(UserVM model, IFormFile? imageFile)
    {
        string role = SessionUtils.GetUser(HttpContext)!.Role!;
        IEnumerable<CountryVM> countries = await _locationService.GetAllCountriesAsync();
        IEnumerable<StateVM> states = await _locationService.GetStatesByCountryIdAsync(model.CountryId);
        IEnumerable<CityVM> cities = await _locationService.GetCitiesByStateIdAsync(model.StateId);
        Response<IEnumerable<RoleVM>> roles = await _roleAndPermissionService.GetAllRolesAsync();

        ViewBag.Countries = new SelectList(countries, "Id", "Name");
        ViewBag.States = new SelectList(states, "Id", "Name");
        ViewBag.Cities = new SelectList(cities, "Id", "Name");
        ViewBag.Roles = new SelectList(roles.Data, "Id", "Name").Where(r => RolePriority.GetRolePriority(r.Text) < RolePriority.GetRolePriority(role));

        ModelState.Remove("Password");
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (imageFile != null)
        {
            string profileImage = await FileUploadHandler.UploadImage(imageFile);
            if (!string.IsNullOrEmpty(profileImage))
            {
                FileUploadHandler.DeleteFile(model.ProfileImage);
                model.ProfileImage = profileImage;
            }
        }
        else if (string.IsNullOrEmpty(model.ProfileImage))
        {
            FileUploadHandler.DeleteFile(model.ProfileImage);
            model.ProfileImage = null;
        }

        Response<UserVM?> response = await _userService.UpdateAsync(model);

        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return View(model);
        }

        TempData["success"] = response.Message;

        return RedirectToAction("Index");
    }

    [PermissionAuthorize(Constants.CanEdit)]
    public async Task<JsonResult> UploadProfileImage(Guid id, IFormFile? imageFile)
    {
        string profileImage = await FileUploadHandler.UploadImage(imageFile);
        if (!string.IsNullOrEmpty(profileImage))
        {
            Response<bool> response = await _userService.UpdateProfileImageAsync(id, profileImage);
            if (response.Success)
            {
                return Json(new { success = true, message = response.Message });
            }
        }
        return Json(new { success = false, message = "User Image not updated!" });
    }

    [PermissionAuthorize(Constants.CanDelete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        Guid currentUserId = (Guid)SessionUtils.GetUserId(HttpContext)!;
        if (currentUserId == id)
        {
            TempData["error"] = "You Can't Delete Yourself!";
            return RedirectToAction("Index");
        }

        Response<bool> response = await _userService.DeleteAsync(id);
        if (!response.Success)
        {
            TempData["error"] = response.Message;
            return RedirectToAction("Index");
        }
        TempData["success"] = response.Message;

        return RedirectToAction("Index");
    }
}
