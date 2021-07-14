using Appel.SharpTemplate.DTOs;
using Appel.SharpTemplate.DTOs.User;
using Appel.SharpTemplate.Infrastructure;
using Appel.SharpTemplate.Models;
using Appel.SharpTemplate.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Controllers
{
    public class UserController : BaseController
    {
        private readonly SharpTemplateContext _context;
        private readonly IOptions<AppSettings> _appSettings;

        public UserController(SharpTemplateContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("authenticate")]
        public async Task<IActionResult> AuthenticateAsync([FromBody] UserAuthenticateDTO user)
        {
            var userService = new UserService(_context, _appSettings);

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

            var userService = new UserService(_context, _appSettings);

            await userService.EditAsync(user);

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterDTO user)
        {
            var userService = new UserService(_context, _appSettings);

            await userService.RegisterAsync(user);

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] string email)
        {
            var userService = new UserService(_context, _appSettings);

            await userService.SendForgotPasswordEmailAsync(email);

            return Ok();
        }

        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] UserChangePasswordDTO user)
        {
            var userService = new UserService(_context, _appSettings);

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

            var userService = new UserService(_context, _appSettings);

            var user = await userService.GetUserByIdAsync(id);

            return Json(user);
        }

        [HttpGet]
        [Route("list-all")]
        [AuthorizeRoles(UserRole.Admin)]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var userService = new UserService(_context, _appSettings);

            var users = await userService.GetAllUsersAsync();

            return Json(users);
        }
    }
}
