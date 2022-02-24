using Appel.SharpTemplate.Domain.Entities;
using Appel.SharpTemplate.Domain.Interfaces;
using Appel.SharpTemplate.Infrastructure.Application;
using Appel.SharpTemplate.Infrastructure.Data.Repositories;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;

namespace Appel.SharpTemplate.UnitTests;

public abstract class DependencyInjectionTest
{
    protected readonly IUserRepository UserRepository;
    protected readonly IOptions<AppSettings> AppSettings;
    protected readonly JsonSerializerOptions JsonSerializerOptions;

    protected DependencyInjectionTest()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.test.json", false, true)
            .Build();

        var services = new ServiceCollection();

        services.AddOptions()
            .AddControllers()
            .AddFluentValidation(fv =>
            {
                fv.ValidatorOptions.PropertyNameResolver = CamelCasePropertyNameResolver.ResolvePropertyName;
            });

        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

        AppSettings = services.BuildServiceProvider().GetRequiredService<IOptions<AppSettings>>();

        var contextFactory = new TestDbContextFactory();
        var repositoryBase = new RepositoryBase<User>(contextFactory);

        UserRepository = new UserRepository(repositoryBase);

        JsonSerializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
