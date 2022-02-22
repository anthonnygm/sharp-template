using Appel.SharpTemplate.DTOs.User;
using Appel.SharpTemplate.Infrastructure;
using Appel.SharpTemplate.Repositories.Abstractions;
using Appel.SharpTemplate.Utils;
using FluentValidation;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Validators.DTOs
{
    public class UserAuthenticateDTOValidator : AbstractValidator<UserAuthenticateDTO>
    {
        private readonly IUserRepository _repository;
        private readonly IOptions<AppSettings> _appSettings;

        public UserAuthenticateDTOValidator(IUserRepository repository, IOptions<AppSettings> appSettings)
        {
            _repository = repository;
            _appSettings = appSettings;

            RuleFor(x => x)
                .MustAsync(BeValidUserAsync).OverridePropertyName("InvalidLogin").WithMessage("Invalid e-mail or password");
        }

        public async Task<bool> BeValidUserAsync(UserAuthenticateDTO user, CancellationToken cancellationToken)
        {
            var argon2HashManager = new Argon2HashManager(_appSettings);

            var databaseUser = (await _repository.GetAsync(x => x.Email == user.Email)).FirstOrDefault();

            return databaseUser != null && argon2HashManager.VerifyPasswordHash(user.Password, databaseUser.Password);
        }
    }
}
