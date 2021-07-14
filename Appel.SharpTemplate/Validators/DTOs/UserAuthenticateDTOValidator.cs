using Appel.SharpTemplate.DTOs.User;
using Appel.SharpTemplate.Models;
using FluentValidation;
using System.Linq;

namespace Appel.SharpTemplate.Validators.DTOs
{
    public class UserAuthenticateDTOValidator : AbstractValidator<UserAuthenticateDTO>
    {
        public UserAuthenticateDTOValidator(SharpTemplateContext databaseContext)
        {
            RuleFor(x => x).Custom((x, validationContext) =>
            {
                if (databaseContext.Users.FirstOrDefault(y => y.Email == x.Email && y.Password == x.Password) == null)
                {
                    validationContext.AddFailure("InvalidLogin", "Invalid e-mail or password");
                }
            });
        }
    }
}
