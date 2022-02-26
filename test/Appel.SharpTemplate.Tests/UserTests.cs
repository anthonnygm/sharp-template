using Appel.SharpTemplate.API.Application.Extensions;
using Appel.SharpTemplate.API.Application.Models;
using Appel.SharpTemplate.API.Application.Services;
using Appel.SharpTemplate.API.Application.Validators.Models;
using Appel.SharpTemplate.Domain.Entities;
using Appel.SharpTemplate.Domain.Interfaces;
using Appel.SharpTemplate.Infrastructure.Data.Repositories;
using AutoMapper;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Appel.SharpTemplate.UnitTests;

public class UserTests : DependencyInjectionTest
{
    #region Authenticate

    [Fact]
    public async Task UserValidate_Authenticate_Success()
    {
        var repository = GetUserRepository();
        var service = new UserService(repository, AppSettings, default);

        var userRegisterViewModel = CreateDefaultUser();

        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<UserRegisterViewModel, UserAuthenticateViewModel>()));
        var authenticateViewModel = mapper.Map<UserAuthenticateViewModel>(userRegisterViewModel);

        await service.RegisterAsync(userRegisterViewModel);

        var validator = new UserAuthenticateViewModelValidator(repository, AppSettings);
        var result = await validator.ValidateAsync(authenticateViewModel);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task UserValidate_Authenticate_Fail()
    {
        var user = new UserAuthenticateViewModel()
        {
            Email = "test_email@gmail.com",
            Password = "12345678"
        };

        var repository = GetUserRepository();

        var validator = new UserAuthenticateViewModelValidator(repository, AppSettings);
        var result = await validator.ValidateAsync(user);

        Assert.False(result.IsValid);
        Assert.Equal("Invalid e-mail or password", result.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task UserValidate_ResetPassword_Success()
    {
        var repository = GetUserRepository();
        var service = new UserService(repository, AppSettings, default);

        var user = CreateDefaultUser();

        await service.RegisterAsync(user);

        var databaseUser = await repository.GetByIdAsync(id: 1);
        var jsonEmailToken = JsonSerializer.Serialize(new EmailTokenViewModel() { Email = databaseUser?.Email, Validity = DateTime.Now.AddHours(3) });

        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<User, UserResetPasswordViewModel>()));

        var userResetPassword = mapper.Map<UserResetPasswordViewModel>(databaseUser);
        userResetPassword.EmailHash = CryptographyExtensions.Encrypt(AppSettings.Value.EmailTokenSecretKey, jsonEmailToken);
        userResetPassword.Password = "p1o2i3u4y5";
        userResetPassword.PasswordConfirmation = "p1o2i3u4y5";

        var validator = new UserResetPasswordViewModelValidator(repository, AppSettings);
        var result = await validator.ValidateAsync(userResetPassword);

        await service.ResetPasswordAsync(userResetPassword);
        var updatedUser = await repository.GetByIdAsync(id: 1);

        Assert.True(result.IsValid);
        Assert.Equal(userResetPassword.Password, updatedUser?.Password);
    }

    [Fact]
    public async Task UserValidate_ResetPassword_Fail()
    {
        var user = new UserResetPasswordViewModel()
        {
            Password = "p1o2i3u4y5",
            PasswordConfirmation = "p1o2i3u4y5"
        };

        var repository = GetUserRepository();

        var validator = new UserResetPasswordViewModelValidator(repository, AppSettings);
        var result = await validator.ValidateAsync(user);

        Assert.False(result.IsValid);
        Assert.Equal("Invalid e-mail hash", result.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task UserValidate_ChangePassword_Success()
    {
        var repository = GetUserRepository();
        var service = new UserService(repository, AppSettings, default);

        var user = CreateDefaultUser();

        await service.RegisterAsync(user);

        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<UserRegisterViewModel, UserChangePasswordViewModel>()));

        var userChangePassword = mapper.Map<UserChangePasswordViewModel>(user);
        userChangePassword.CurrentPassword = user.Password;
        userChangePassword.NewPassword = "p1o2i3u4y5";
        userChangePassword.NewPasswordConfirmation = "p1o2i3u4y5";

        var validator = new UserChangePasswordViewModelValidator(repository);
        var result = await validator.ValidateAsync(userChangePassword);

        await service.ChangePasswordAsync(userChangePassword);
        var updatedUser = await repository.GetByIdAsync(1);

        Assert.True(result.IsValid);
        Assert.Equal(userChangePassword.NewPassword, updatedUser.Password);
    }

    [Fact]
    public async Task UserValidate_ChangePassword_Fail()
    {
        var repository = GetUserRepository(Guid.NewGuid().ToString());
        var service = new UserService(repository, AppSettings, default);

        var user = CreateDefaultUser();

        await service.RegisterAsync(user);

        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.CreateMap<UserRegisterViewModel, UserChangePasswordViewModel>()));

        var userChangePassword = mapper.Map<UserChangePasswordViewModel>(user);
        userChangePassword.Email = user.Email;
        userChangePassword.CurrentPassword = "p1o2i3u4y5";
        userChangePassword.NewPassword = "p1o2i3u4y5";
        userChangePassword.NewPasswordConfirmation = "p1o2i3u4y5";

        var validator = new UserChangePasswordViewModelValidator(repository);
        var result = await validator.ValidateAsync(userChangePassword);

        Assert.False(result.IsValid);
        Assert.Equal("Invalid password", result.Errors.First().ErrorMessage);
    }

    #endregion

    #region Legal User

    [Fact]
    public async Task UserLegal_Register_InvalidPassword()
    {
        var repository = GetUserRepository();
        var service = new UserService(repository, AppSettings, default);

        var user = CreateDefaultLegalUser();
        user.Password = string.Empty;
        user.PasswordConfirmation = string.Empty;

        var validator = new UserRegisterViewModelValidator(repository);
        var result = await validator.ValidateAsync(user);

        var passwordErrors = new Queue<ValidationFailure>(result.Errors.Where(x => x.PropertyName == "password"));
        var passwordConfirmationErrors = new Queue<ValidationFailure>(result.Errors.Where(x => x.PropertyName == "passwordConfirmation"));

        Assert.False(result.IsValid);

        Assert.Equal("Required field", passwordErrors.Dequeue().ErrorMessage);
        Assert.Equal("Password must contain at least 1 number", passwordErrors.Dequeue().ErrorMessage);
        Assert.Equal("Password must contain at least 1 letter", passwordErrors.Dequeue().ErrorMessage);
        Assert.Equal("The password must contain between 8 and 20 digits", passwordErrors.Dequeue().ErrorMessage);

        Assert.Equal("Required field", passwordConfirmationErrors.Dequeue().ErrorMessage);
    }

    [Fact]
    public async Task UserLegal_Register_PasswordNotMatch()
    {
        var repository = GetUserRepository(Guid.NewGuid().ToString());

        var user = CreateDefaultLegalUser();
        user.PasswordConfirmation = "q1w2e3r45";

        var validator = new UserRegisterViewModelValidator(repository);
        var result = await validator.ValidateAsync(user);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.All(x => x.ErrorMessage == "Passwords doesn't match"));
    }

    [Fact]
    public async Task UserLegal_Register_InvalidCnpj()
    {
        var repository = GetUserRepository(Guid.NewGuid().ToString());

        var user = CreateDefaultLegalUser();
        user.CpfCnpj = "123";

        var validator = new UserRegisterViewModelValidator(repository);
        var result = await validator.ValidateAsync(user);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.All(x => x.ErrorMessage == "Invalid CNPJ"));
    }

    [Theory]
    [InlineData("test_email")]
    [InlineData("test_email@")]
    [InlineData("test_email.com")]
    public async Task UserLegal_Register_InvalidEmail(string email)
    {
        var repository = GetUserRepository();

        var user = CreateDefaultLegalUser();
        user.Email = email;

        var validator = new UserRegisterViewModelValidator(repository);
        var result = await validator.ValidateAsync(user);

        Assert.False(result.IsValid);
        Assert.Equal("Invalid email address", result.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task UserLegal_Register_NotUnique()
    {
        var repository = GetUserRepository();
        var service = new UserService(repository, AppSettings, default);

        var user = CreateDefaultLegalUser();

        await service.RegisterAsync(user);

        var validator = new UserRegisterViewModelValidator(repository);
        var result = await validator.ValidateAsync(user);

        var queue = new Queue<ValidationFailure>(result.Errors);

        Assert.False(result.IsValid);
        Assert.Equal("E-mail already registered", queue.Dequeue().ErrorMessage);
        Assert.Equal("CNPJ already registered", queue.Dequeue().ErrorMessage);
    }

    [Fact]
    public async Task UserLegal_Register_NumbersOnly_Success()
    {
        var repository = GetUserRepository(Guid.NewGuid().ToString());

        var user = CreateDefaultLegalUser();
        user.CpfCnpj = "19.939.319/0001-05";
        user.ResponsibleCpf = "974.286.430-66";
        user.StateRegistration = "485/4916850";
        user.Phone = "(51)3049-0235";
        user.CellPhone = "(51)99237-1701";
        user.ZipCode = "93700-000";

        var json = JsonSerializer.Serialize(user, JsonSerializerOptions);
        user = JsonSerializer.Deserialize<UserRegisterViewModel>(json, JsonSerializerOptions);

        var validator = new UserRegisterViewModelValidator(repository);
        var result = await validator.ValidateAsync(user);

        Assert.True(result.IsValid);
        Assert.Equal("19939319000105", user.CpfCnpj);
        Assert.Equal("97428643066", user.ResponsibleCpf);
        Assert.Equal("4854916850", user.StateRegistration);
        Assert.Equal("5130490235", user.Phone);
        Assert.Equal("51992371701", user.CellPhone);
        Assert.Equal("93700000", user.ZipCode);
    }

    [Fact]
    public async Task UserLegal_Register_Success()
    {
        var user = CreateDefaultLegalUser();

        var repository = GetUserRepository();

        var validator = new UserRegisterViewModelValidator(repository);
        var result = await validator.ValidateAsync(user);

        Assert.True(result.IsValid);
    }

    #endregion

    #region Physical User

    [Fact]
    public async Task UserPhysical_Register_InvalidPassword()
    {
        var repository = GetUserRepository();

        var user = CreateDefaultPhysicalUser();
        user.Password = string.Empty;
        user.PasswordConfirmation = string.Empty;

        var validator = new UserRegisterViewModelValidator(repository);
        var result = await validator.ValidateAsync(user);

        var passwordErrors = new Queue<ValidationFailure>(result.Errors.Where(x => x.PropertyName == "password"));
        var passwordConfirmationErrors = new Queue<ValidationFailure>(result.Errors.Where(x => x.PropertyName == "passwordConfirmation"));

        Assert.False(result.IsValid);

        Assert.Equal("Required field", passwordErrors.Dequeue().ErrorMessage);
        Assert.Equal("Password must contain at least 1 number", passwordErrors.Dequeue().ErrorMessage);
        Assert.Equal("Password must contain at least 1 letter", passwordErrors.Dequeue().ErrorMessage);
        Assert.Equal("The password must contain between 8 and 20 digits", passwordErrors.Dequeue().ErrorMessage);

        Assert.Equal("Required field", passwordConfirmationErrors.Dequeue().ErrorMessage);
    }

    [Fact]
    public async Task UserPhysical_Register_PasswordNotMatch()
    {
        var repository = GetUserRepository(Guid.NewGuid().ToString());

        var user = CreateDefaultPhysicalUser();
        user.PasswordConfirmation = "q1w2e3r45";

        var validator = new UserRegisterViewModelValidator(repository);
        var result = await validator.ValidateAsync(user);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.All(x => x.ErrorMessage == "Passwords doesn't match"));
    }

    [Fact]
    public async Task UserPhysical_Register_InvalidCpf()
    {
        var repository = GetUserRepository(Guid.NewGuid().ToString());

        var user = CreateDefaultPhysicalUser();
        user.CpfCnpj = "123";

        var validator = new UserRegisterViewModelValidator(repository);
        var result = await validator.ValidateAsync(user);

        Assert.False(result.IsValid);
        Assert.True(result.Errors.All(x => x.ErrorMessage == "Invalid CPF"));
    }

    [Theory]
    [InlineData("test_email")]
    [InlineData("test_email@")]
    [InlineData("test_email.com")]
    public async Task UserPhysical_Register_InvalidEmail(string email)
    {
        var repository = GetUserRepository();

        var user = CreateDefaultPhysicalUser();
        user.Email = email;

        var validator = new UserRegisterViewModelValidator(repository);
        var result = await validator.ValidateAsync(user);

        Assert.False(result.IsValid);
        Assert.Equal("Invalid email address", result.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task UserPhysical_Register_NotUnique()
    {
        var repository = GetUserRepository();
        var service = new UserService(repository, AppSettings, default);

        var user = CreateDefaultPhysicalUser();
        await service.RegisterAsync(user);

        var validator = new UserRegisterViewModelValidator(repository);
        var result = await validator.ValidateAsync(user);

        var queue = new Queue<ValidationFailure>(result.Errors);

        Assert.False(result.IsValid);
        Assert.Equal("E-mail already registered", queue.Dequeue().ErrorMessage);
        Assert.Equal("CPF already registered", queue.Dequeue().ErrorMessage);
    }

    [Fact]
    public async Task UserPhisical_Register_NumbersOnly_Success()
    {
        var repository = GetUserRepository(Guid.NewGuid().ToString());

        var user = CreateDefaultPhysicalUser();
        user.CpfCnpj = "974.286.430-66";
        user.Phone = "(51)3049-0235";
        user.CellPhone = "(51)99237-1701";
        user.ZipCode = "93700-000";

        var json = JsonSerializer.Serialize(user, JsonSerializerOptions);
        user = JsonSerializer.Deserialize<UserRegisterViewModel>(json, JsonSerializerOptions);

        var validator = new UserRegisterViewModelValidator(repository);
        var result = await validator.ValidateAsync(user);

        Assert.True(result.IsValid);
        Assert.Equal("97428643066", user.CpfCnpj);
        Assert.Equal("5130490235", user.Phone);
        Assert.Equal("51992371701", user.CellPhone);
        Assert.Equal("93700000", user.ZipCode);
    }

    [Fact]
    public async Task UserPhysical_Register_Success()
    {
        var repository = GetUserRepository();

        var user = CreateDefaultPhysicalUser();

        var validator = new UserRegisterViewModelValidator(repository);
        var result = await validator.ValidateAsync(user);

        Assert.True(result.IsValid);
    }

    #endregion

    #region Generic User

    [Fact]
    public async Task GenericUser_GetUserById_Success()
    {
        var repository = GetUserRepository();
        var service = new UserService(repository, AppSettings, default);

        var user = CreateDefaultUser();
        await service.RegisterAsync(user);

        var databaseUser = await service.GetByIdAsync(id: 1);

        Assert.NotNull(databaseUser);
        Assert.Equal(user.Email, databaseUser.Email);
    }

    [Fact]
    public async Task GenericUser_GetAllUsers_Success()
    {
        var repository = GetUserRepository();
        var service = new UserService(repository, AppSettings, default);

        var user = CreateDefaultUser();
        await service.RegisterAsync(user);

        var databaseUsers = await service.GetAsync();

        Assert.NotNull(databaseUsers);
        Assert.NotEmpty(databaseUsers);
        Assert.Equal(user.Email, databaseUsers.First().Email);
    }

    [Fact]
    public async Task GenericUser_EditUser_Success()
    {
        var repository = GetUserRepository();
        var service = new UserService(repository, AppSettings, default);

        var user = CreateDefaultUser();
        await service.RegisterAsync(user);

        user.City = "Zochester";
        user.Neighborhood = "Troit";

        var config = new MapperConfiguration(cfg => cfg.CreateMap<UserRegisterViewModel, UserProfileViewModel>());
        var mapper = new Mapper(config);

        var userProfileViewModel = mapper.Map<UserProfileViewModel>(user);
        userProfileViewModel.Id = 1;

        await service.UpdateAsync(userProfileViewModel);

        var databaseUser = await service.GetByIdAsync(1);

        Assert.Equal(userProfileViewModel.Name, databaseUser.Name);
        Assert.Equal(userProfileViewModel.Email, databaseUser.Email);
        Assert.Equal(userProfileViewModel.City, databaseUser.City);
        Assert.Equal(userProfileViewModel.Neighborhood, databaseUser.Neighborhood);
    }

    #endregion

    #region Auxiliary Methods

    private static UserRegisterViewModel CreateDefaultUser()
    {
        return new UserRegisterViewModel()
        {
            Name = "Test Name",
            Email = "test_email@gmail.com",
            Password = "q1w2e3r4",
            PasswordConfirmation = "q1w2e3r4",
            CpfCnpj = "86856740000",
            CellPhone = "999999999",
            Address = "River Gait",
            AddressNumber = "123",
            Neighborhood = "Rockville",
            City = "Oseland",
            ZipCode = "93700000",
            FederativeUnit = "RS"
        };
    }

    private static UserRegisterViewModel CreateDefaultLegalUser()
    {
        var user = CreateDefaultUser();
        user.CpfCnpj = "08634417000118";
        user.ResponsibleCpf = "86856740000";
        user.ResponsibleName = "Test Responsible Name";
        user.StateRegistration = "485/4916850";
        user.Type = UserType.Legal;

        return user;
    }

    private static UserRegisterViewModel CreateDefaultPhysicalUser()
    {
        var user = CreateDefaultUser();
        user.IdentityDocument = "235789331";

        return user;
    }

    private static IUserRepository GetUserRepository(string databaseName = "test-db")
    {
        var contextFactory = new TestDbContextFactory(databaseName);
        var repositoryBase = new RepositoryBase<User>(contextFactory);

        return new UserRepository(repositoryBase);
    }

    #endregion
}
