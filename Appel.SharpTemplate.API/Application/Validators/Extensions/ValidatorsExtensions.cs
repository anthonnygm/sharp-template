using FluentValidation;

namespace Appel.SharpTemplate.API.Application.Validators.Extensions
{
    public static class ValidatorsExtensions
    {
        public static IRuleBuilderOptions<T, string> IsRequired<T>(this IRuleBuilder<T, string> rule) => rule.NotEmpty().WithMessage("Required field");

        public static IRuleBuilderOptions<T, string> PasswordValidation<T>(this IRuleBuilder<T, string> rule)
        {
            return rule
                .IsRequired()
                .Matches("[0-9]").WithMessage("Password must contain at least 1 number")
                .Matches("[A-Za-z]").WithMessage("Password must contain at least 1 letter")
                .Matches(".{8,20}").WithMessage("The password must contain between 8 and 20 digits");
        }
    }
}
