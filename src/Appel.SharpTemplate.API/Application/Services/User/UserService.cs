using Appel.SharpTemplate.API.Application.Extensions;
using Appel.SharpTemplate.API.Application.Interfaces;
using Appel.SharpTemplate.API.Application.Models;
using Appel.SharpTemplate.Domain.Entities;
using Appel.SharpTemplate.Domain.Interfaces;
using Appel.SharpTemplate.Infrastructure.Application;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.API.Application.Services;

public class UserService : EmailBase, IUserService
{
    private readonly IUserRepository _repository;
    private readonly IEmailService _emailService;
    private readonly IOptions<AppSettings> _appSettings;

    public UserService(IUserRepository repository, IOptions<AppSettings> appSettings, IEmailService emailService)
    {
        _repository = repository;
        _appSettings = appSettings;
        _emailService = emailService;
    }

    public async Task<UserTokenAuthViewModel> AuthenticateAsync(UserAuthenticateViewModel user)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(_appSettings.Value.AuthTokenSecretKey);

        var databaseUser = (await _repository.GetAsync(x => x.Email == user.Email)).FirstOrDefault();

        if (databaseUser == null)
        {
            return new UserTokenAuthViewModel();
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, databaseUser.Id.ToString()),
                new Claim(ClaimTypes.Role, databaseUser.Role.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);

        return new UserTokenAuthViewModel()
        {
            UserId = databaseUser.Id,
            Token = jwtTokenHandler.WriteToken(token)
        };
    }

    public async Task UpdateAsync(UserProfileViewModel user)
    {
        var config = new MapperConfiguration(cfg => cfg.CreateMap<UserProfileViewModel, User>());
        var mapper = new Mapper(config);

        var mappedUser = mapper.Map<User>(user);

        await _repository.UpdateAsync(mappedUser);
    }

    public async Task RegisterAsync(UserRegisterViewModel user)
    {
        var argon2HashManager = new Argon2HashManager(_appSettings);

        var hashPassword = argon2HashManager.CreatePasswordHash(user.Password);

        user.Password = hashPassword;
        user.PasswordConfirmation = hashPassword;

        var config = new MapperConfiguration(cfg => cfg.CreateMap<UserRegisterViewModel, User>());
        var mapper = new Mapper(config);

        await _repository.AddAsync(mapper.Map<User>(user));
    }

    public async Task ResetPasswordAsync(UserResetPasswordViewModel user)
    {
        var databaseUser = await _repository.GetByIdAsync(user.Id);
        databaseUser.Password = user.Password;

        await _repository.UpdateAsync(databaseUser);
    }

    public async Task ChangePasswordAsync(UserChangePasswordViewModel user)
    {
        var databaseUser = (await _repository.GetAsync(x => x.Email == user.Email)).FirstOrDefault();

        if (databaseUser == null)
        {
            return;
        }

        databaseUser.Password = user.NewPassword;

        await _repository.UpdateAsync(databaseUser);
    }

    public async Task<UserProfileViewModel> GetByIdAsync(int id)
    {
        var databaseUser = await _repository.GetByIdAsync(id);

        var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserProfileViewModel>());
        var mapper = new Mapper(config);

        return mapper.Map<UserProfileViewModel>(databaseUser);
    }

    public async Task<IEnumerable<UserProfileViewModel>> GetAsync()
    {
        var databaseUsers = await _repository.GetAsync();

        var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserProfileViewModel>());
        var mapper = new Mapper(config);

        return mapper.Map<IEnumerable<UserProfileViewModel>>(databaseUsers);
    }

    public async Task SendForgotPasswordEmailAsync(string email)
    {
        var databaseUser = (await _repository.GetAsync(x => x.Email == email)).FirstOrDefault();

        if (databaseUser == null)
        {
            return;
        }

        var message = await LoadEmailTemplateAsync("email-reset-password");

        var jsonEmailToken = JsonSerializer.Serialize(new EmailTokenViewModel() { Email = email, Validity = DateTime.Now.AddHours(3) });
        var emailHash = CryptographyExtensions.Encrypt(_appSettings.Value.EmailTokenSecretKey, jsonEmailToken);

        message = message
            .Replace("{userId}", databaseUser.Id.ToString())
            .Replace("{emailHash}", emailHash);

        await _emailService.SendAsync("Sharp Template - Reset Password", message, email);
    }
}
