using Appel.SharpTemplate.API.Application.DTOs.User;
using Appel.SharpTemplate.API.Application.Validators.Extensions;
using Appel.SharpTemplate.Domain.Interfaces;
using FluentValidation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.API.Application.Validators.DTOs;

public class UserChangePasswordDTOValidator : AbstractValidator<UserChangePasswordDTO>
{
    private readonly IUserRepository _repository;

    public UserChangePasswordDTOValidator(IUserRepository repository)
    {
        _repository = repository;

        RuleFor(x => x.Email)
            .IsRequired();

        RuleFor(x => x)
            .MustAsync(BeValidPasswordAsync).WithMessage("Invalid password");

        RuleFor(x => x.NewPassword)
            .PasswordValidation();

        RuleFor(x => x.NewPasswordConfirmation)
            .IsRequired()
            .Equal(x => x.NewPassword).WithMessage("Passwords doesn't match");
    }

    public async Task<bool> BeValidPasswordAsync(UserChangePasswordDTO user, CancellationToken cancellationToken)
    {
        return (await _repository.GetAsync(x => x.Email == user.Email && x.Password == user.CurrentPassword)).Any();
    }
}
