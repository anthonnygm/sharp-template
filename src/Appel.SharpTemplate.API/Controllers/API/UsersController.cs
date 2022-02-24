using Appel.SharpTemplate.API.Application.DTOs.User;
using Appel.SharpTemplate.API.Application.Services;
using Appel.SharpTemplate.Domain.Entities;
using Appel.SharpTemplate.Domain.Interfaces;
using Appel.SharpTemplate.Infrastructure.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.API.Controllers.API;

public class UsersController : BaseController
{
    private readonly IUserRepository _repository;
    private readonly IOptions<AppSettings> _appSettings;

    public UsersController(IUserRepository repository, IOptions<AppSettings> appSettings)
    {
        _repository = repository;
        _appSettings = appSettings;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("authenticate")]
    public async Task<IActionResult> AuthenticateAsync([FromBody] UserAuthenticateDTO user)
    {
        var userService = new UserService(_repository, _appSettings);

        var token = await userService.AuthenticateAsync(user);

        return Json(token);
    }

    [HttpPost]
    [Route("edit")]
    public async Task<IActionResult> EditAsync([FromBody] UserProfileDTO user)
    {
        var userTokenId = int.Parse(User.Identity?.Name ?? string.Empty);

        if (user.Id != userTokenId && !User.IsInRole(UserRole.Admin.ToString()))
        {
            return Forbid();
        }

        var userService = new UserService(_repository, _appSettings);

        await userService.EditAsync(user);

        return Ok();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterDTO user)
    {
        var userService = new UserService(_repository, _appSettings);

        await userService.RegisterAsync(user);

        return Ok();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("forgot-password")]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] string email)
    {
        var userService = new UserService(_repository, _appSettings);

        await userService.SendForgotPasswordEmailAsync(email);

        return Ok();
    }

    [HttpPost]
    [Route("change-password")]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] UserChangePasswordDTO user)
    {
        var userService = new UserService(_repository, _appSettings);

        await userService.ChangePasswordAsync(user);

        return Ok();
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetUserByIdAsync(int id)
    {
        var userTokenId = int.Parse(User.Identity?.Name ?? string.Empty);

        if (id != userTokenId && !User.IsInRole(UserRole.Admin.ToString()))
        {
            return Forbid();
        }

        var userService = new UserService(_repository, _appSettings);

        var user = await userService.GetUserByIdAsync(id);

        return Json(user);
    }

    [HttpGet]
    [Route("list-all")]
    [AuthorizeRoles(UserRole.Admin)]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var userService = new UserService(_repository, _appSettings);

        var users = await userService.GetAllUsersAsync();

        return Json(users);
    }
}
