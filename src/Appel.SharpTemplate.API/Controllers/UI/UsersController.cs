using Appel.SharpTemplate.API.Application.Interfaces;
using Appel.SharpTemplate.API.Application.Models;
using Appel.SharpTemplate.Infrastructure.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.API.Controllers.UI;

public class UsersController : BaseController
{
    private readonly IUserService _userService;
    private readonly IOptionsMonitor<AppSettings> _appSettings;

    public UsersController(IUserService userService, IOptionsMonitor<AppSettings> appSettings)
    {
        _userService = userService;
        _appSettings = appSettings;
    }

    [HttpPost]
    [Route("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync([FromForm] UserResetPasswordViewModel user)
    {
        if (!ModelState.IsValid)
        {
            var errorList = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            if (errorList.Any(x => x.Key != "Password" && x.Key != "PasswordConfirmation"))
            {
                return ViewError("There was an error changing the password, check if the link was entered correctly.");
            }

            return View();
        }

        await _userService.ResetPasswordAsync(user);

        ViewBag.Message = "Password reset successfully!";

        return View();
    }

    [HttpGet]
    [Route("reset-password")]
    public IActionResult ResetPasswordAsync()
    {
        var id = int.Parse(Request.Query["id"]);
        var hash = Request.Query["hash"];

        return View("ResetPassword", new UserResetPasswordViewModel()
        {
            Id = id,
            EmailHash = hash
        });
    }
}
