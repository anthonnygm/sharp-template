using Appel.SharpTemplate.API.Application.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.API.Application.Interfaces;

public interface IUserService
{
    Task<UserTokenAuthViewModel> AuthenticateAsync(UserAuthenticateViewModel user);
    Task UpdateAsync(UserProfileViewModel user);
    Task RegisterAsync(UserRegisterViewModel user);
    Task SendForgotPasswordEmailAsync(string email);
    Task ResetPasswordAsync(UserResetPasswordViewModel user);
    Task ChangePasswordAsync(UserChangePasswordViewModel user);
    Task<UserProfileViewModel> GetByIdAsync(int id);
    Task<IEnumerable<UserProfileViewModel>> GetAsync();
}
