namespace Appel.SharpTemplate.API.Application.DTOs.User
{
    public class UserChangePasswordDTO
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirmation { get; set; }
    }
}
