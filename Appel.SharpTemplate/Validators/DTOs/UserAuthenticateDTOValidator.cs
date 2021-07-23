using Appel.SharpTemplate.DTOs.User;
using Appel.SharpTemplate.Infrastructure;
using Appel.SharpTemplate.Models;
using Appel.SharpTemplate.Utils;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Validators.DTOs
{
    public class UserAuthenticateDTOValidator : AbstractValidator<UserAuthenticateDTO>
    {
        private readonly SharpTemplateContext _context;
        private readonly IOptions<AppSettings> _appSettings;

        public UserAuthenticateDTOValidator(SharpTemplateContext databaseContext, IOptions<AppSettings> appSettings)
        {
            _context = databaseContext;
            _appSettings = appSettings;

            RuleFor(x => x)
                .MustAsync(BeValidUserAsync).OverridePropertyName("InvalidLogin").WithMessage("Invalid e-mail or password");
        }

        public async Task<bool> BeValidUserAsync(UserAuthenticateDTO user, CancellationToken cancellationToken)
        {
            var argon2HashManager = new Argon2HashManager(_appSettings);

            var databaseUser = await _context.Users.FirstOrDefaultAsync(y => y.Email == user.Email, cancellationToken);

            return databaseUser != null && argon2HashManager.VerifyPasswordHash(user.Password, databaseUser.Password);
        }
    }
}
