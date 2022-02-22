using Appel.SharpTemplate.API.Application.DTOs.Email;
using Appel.SharpTemplate.API.Application.Validators.Extensions;
using Appel.SharpTemplate.API.Extensions;
using Appel.SharpTemplate.API.ViewModels;
using Appel.SharpTemplate.Domain.Interfaces;
using Appel.SharpTemplate.Infrastructure.Application;
using FluentValidation;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.API.Application.Validators.ViewModels
{
    public class UserResetPasswordViewModelValidator : AbstractValidator<ResetPassword>
    {
        private readonly IUserRepository _repository;
        private readonly IOptions<AppSettings> _appSettings;

        public UserResetPasswordViewModelValidator(IUserRepository repository, IOptions<AppSettings> appSettings)
        {
            _repository = repository;
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
            var databaseUser = await _repository.GetByIdAsync(user.Id);

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
