using Appel.SharpTemplate.Infrastructure;
using Appel.SharpTemplate.Models;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;


namespace Appel.SharpTemplate.Tests
{
    public abstract class DependencyInjectionTest
    {
        protected readonly IOptions<AppSettings> AppSettings;
        protected readonly JsonSerializerOptions JsonSerializerOptions;
        protected readonly DbContextOptions<SharpTemplateContext> DbContextOptions;

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

            DbContextOptions = new DbContextOptionsBuilder<SharpTemplateContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            JsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            };
        }
    }
}
