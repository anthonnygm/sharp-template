﻿namespace Appel.SharpTemplate.API.Application.Models;

public class UserChangePasswordViewModel
{
    public string Email { get; set; }
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string NewPasswordConfirmation { get; set; }
}
