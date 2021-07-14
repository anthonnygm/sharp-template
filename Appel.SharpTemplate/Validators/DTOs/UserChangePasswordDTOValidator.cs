using Appel.SharpTemplate.DTOs.User;
using Appel.SharpTemplate.Models;
using Appel.SharpTemplate.Validators.Extensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Validators.DTOs
{
    public class UserChangePasswordDTOValidator : AbstractValidator<UserChangePasswordDTO>
    {
        private readonly SharpTemplateContext _context;

        public UserChangePasswordDTOValidator(SharpTemplateContext context)
        {
            _context = context;

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
            return await _context.Users.AnyAsync(x => x.Email == user.Email && x.Password == user.CurrentPassword, cancellationToken);
        }
    }
}
