using Appel.SharpTemplate.DTOs;
using Appel.SharpTemplate.DTOs.User;
using Appel.SharpTemplate.Infrastructure;
using Appel.SharpTemplate.Models;
using Appel.SharpTemplate.Utils;
using Appel.SharpTemplate.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Appel.SharpTemplate.Services
{
    public class UserService
    {
        private readonly SharpTemplateContext _context;
        private readonly IOptions<AppSettings> _appSettings;

        public UserService(SharpTemplateContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings;
        }

        public async Task<UserTokenAuthDTO> AuthenticateAsync(UserAuthenticateDTO user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.Value.AuthTokenSecretKey);

            var databaseUser = await _context.Users.FirstAsync(x => x.Email == user.Email);

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

            var databaseUser = await _context.Users.FirstAsync(x => x.Id == user.Id);
            var updatedUser = mapper.Map(mappedUser, databaseUser);

            _context.Users.Update(updatedUser);
            await _context.SaveChangesAsync();
        }

        public async Task RegisterAsync(UserRegisterDTO user)
        {
            var argon2HashManager = new Argon2HashManager(_appSettings);

            var hashPassword = argon2HashManager.CreatePasswordHash(user.Password);

            user.Password = hashPassword;
            user.PasswordConfirmation = hashPassword;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<UserRegisterDTO, User>());
            var mapper = new Mapper(config);

            _context.Users.Add(mapper.Map<User>(user));
            await _context.SaveChangesAsync();
        }

        public async Task SendForgotPasswordEmailAsync(string email)
        {
            var databaseUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (databaseUser == null)
            {
                return;
            }

            var emailService = new EmailSenderService(_appSettings);
            await emailService.SendForgotPasswordEmailAsync(databaseUser.Id, email);
        }

        public async Task ResetPasswordAsync(ResetPassword user)
        {
            var databaseUser = await _context.Users.FirstAsync(x => x.Id == user.Id);

            databaseUser.Password = user.Password;
            await _context.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(UserChangePasswordDTO user)
        {
            var databaseUser = await _context.Users.FirstAsync(x => x.Email == user.Email);

            databaseUser.Password = user.NewPassword;
            await _context.SaveChangesAsync();
        }

        public async Task<UserProfileDTO> GetUserByIdAsync(int id)
        {
            var databaseUser = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserProfileDTO>());
            var mapper = new Mapper(config);

            return mapper.Map<UserProfileDTO>(databaseUser);
        }

        public async Task<IEnumerable<UserProfileDTO>> GetAllUsersAsync()
        {
            var databaseUsers = await _context.Users.ToListAsync();

            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserProfileDTO>());
            var mapper = new Mapper(config);

            return mapper.Map<IEnumerable<UserProfileDTO>>(databaseUsers);
        }
    }
}
