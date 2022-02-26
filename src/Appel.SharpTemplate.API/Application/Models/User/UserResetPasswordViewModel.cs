namespace Appel.SharpTemplate.API.Application.Models;

public class UserResetPasswordViewModel
{
    public int Id { get; set; }
    public string EmailHash { get; set; }
    public string Password { get; set; }
    public string PasswordConfirmation { get; set; }
}
