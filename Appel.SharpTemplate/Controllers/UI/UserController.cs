﻿using Appel.SharpTemplate.Infrastructure;
using Appel.SharpTemplate.Models;
using Appel.SharpTemplate.Services;
using Appel.SharpTemplate.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Controllers.UI
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
        [Route("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromForm] ResetPassword user)
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

            var userService = new UserService(_context, _appSettings);

            await userService.ResetPasswordAsync(user);

            ViewBag.Message = "Password reset successfully!";

            return View();
        }

        [HttpGet]
        [Route("reset-password")]
        public IActionResult ResetPasswordAsync()
        {
            var id = int.Parse(Request.Query["id"]);
            var hash = Request.Query["hash"];

            return View("ResetPassword", new ResetPassword()
            {
                Id = id,
                EmailHash = hash
            });
        }
    }
}