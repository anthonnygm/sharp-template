using Appel.SharpTemplate.DTOs.Email;
using Appel.SharpTemplate.Infrastructure;
using Appel.SharpTemplate.Models;
using Appel.SharpTemplate.Utils;
using Appel.SharpTemplate.Validators.Extensions;
using Appel.SharpTemplate.ViewModels;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Validators.ViewModels
{
    public class UserResetPasswordViewModelValidator : AbstractValidator<ResetPassword>
    {
        private readonly SharpTemplateContext _context;
        private readonly IOptions<AppSettings> _appSettings;

        public UserResetPasswordViewModelValidator(SharpTemplateContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings;

            RuleFor(x => x)
                .MustAsync(BeValidHashAsync).WithMessage("Invalid e-mail hash");

            RuleFor(x => x.Password)
                .PasswordValidation();

            RuleFor(x => x.PasswordConfirmation)
                .IsRequired()
                .Equal(x => x.Password).WithMessage("Passwords doesn't match");
        }


        public async Task<bool> BeValidHashAsync(ResetPassword user, CancellationToken cancellationToken)
        {
            var databaseUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == user.Id, cancellationToken);

            if (databaseUser == null)
            {
                return false;
            }

            var decryptedData = CryptographyExtensions.Decrypt(_appSettings.Value.EmailTokenSecretKey, user.EmailHash);
            var emailToken = JsonSerializer.Deserialize<EmailTokenDTO>(decryptedData);

            return emailToken?.Email == databaseUser.Email && emailToken?.Validity >= DateTime.Now;
        }
    }
}
