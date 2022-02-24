using Appel.SharpTemplate.API.Application.DTOs.User;
using Appel.SharpTemplate.API.Extensions;
using Appel.SharpTemplate.Domain.Interfaces;
using Appel.SharpTemplate.Infrastructure.Application;
using FluentValidation;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.API.Application.Validators.DTOs;

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
