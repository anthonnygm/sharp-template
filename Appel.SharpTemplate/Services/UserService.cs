using Appel.SharpTemplate.DTOs;
using Appel.SharpTemplate.DTOs.User;
using Appel.SharpTemplate.Infrastructure;
using Appel.SharpTemplate.Models;
using Appel.SharpTemplate.Repositories.Abstractions;
using Appel.SharpTemplate.Utils;
using Appel.SharpTemplate.ViewModels;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Services
{
    public class UserService
    {
        private readonly IUserRepository _repository;
        private readonly IOptions<AppSettings> _appSettings;

        public UserService(IUserRepository repository, IOptions<AppSettings> appSettings)
        {
            _repository = repository;
            _appSettings = appSettings;
        }

        public async Task<UserTokenAuthDTO> AuthenticateAsync(UserAuthenticateDTO user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.Value.AuthTokenSecretKey);

            var databaseUser = (await _repository.GetAsync(x => x.Email == user.Email)).FirstOrDefault();

            if (databaseUser == null)
            {
                return new UserTokenAuthDTO();
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

            return new UserTokenAuthDTO()
            {
                UserId = databaseUser.Id,
                Token = jwtTokenHandler.WriteToken(token)
            };
        }

        public async Task EditAsync(UserProfileDTO user)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserProfileDTO, User>());
            var mapper = new Mapper(config);

            var mappedUser = mapper.Map<User>(user);

            config = new MapperConfiguration(cfg => cfg.CreateMap<User, User>()
                .ForAllMembers(opts => opts.Condition((_, _, srcMember) => srcMember != null)));

            mapper = new Mapper(config);

            var databaseUser = await _repository.GetByIdAsync(user.Id);
            var updatedUser = mapper.Map(mappedUser, databaseUser);

            await _repository.UpdateAsync(updatedUser);
        }

        public async Task RegisterAsync(UserRegisterDTO user)
        {
            var argon2HashManager = new Argon2HashManager(_appSettings);

            var hashPassword = argon2HashManager.CreatePasswordHash(user.Password);

            user.Password = hashPassword;
            user.PasswordConfirmation = hashPassword;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserRegisterDTO, User>());
            var mapper = new Mapper(config);

            await _repository.AddAsync(mapper.Map<User>(user));
        }

        public async Task SendForgotPasswordEmailAsync(string email)
        {
            var databaseUser = (await _repository.GetAsync(x => x.Email == email)).FirstOrDefault();

            if (databaseUser == null)
            {
                return;
            }

            var emailService = new EmailSenderService(_appSettings);
            await emailService.SendForgotPasswordEmailAsync(databaseUser.Id, email);
        }

        public async Task ResetPasswordAsync(ResetPassword user)
        {
            var databaseUser = await _repository.GetByIdAsync(user.Id);
            databaseUser.Password = user.Password;

            await _repository.UpdateAsync(databaseUser);
        }

        public async Task ChangePasswordAsync(UserChangePasswordDTO user)
        {
            var databaseUser = (await _repository.GetAsync(x => x.Email == user.Email)).FirstOrDefault();

            if (databaseUser == null)
            {
                return;
            }

            databaseUser.Password = user.NewPassword;

            await _repository.UpdateAsync(databaseUser);
        }

        public async Task<UserProfileDTO> GetUserByIdAsync(int id)
        {
            var databaseUser = await _repository.GetByIdAsync(id);

            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserProfileDTO>());
            var mapper = new Mapper(config);

            return mapper.Map<UserProfileDTO>(databaseUser);
        }

        public async Task<IEnumerable<UserProfileDTO>> GetAllUsersAsync()
        {
            var databaseUsers = await _repository.GetAsync();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserProfileDTO>());
            var mapper = new Mapper(config);

            return mapper.Map<IEnumerable<UserProfileDTO>>(databaseUsers);
        }
    }
}
