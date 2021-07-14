using Appel.SharpTemplate.Infrastructure;
using Appel.SharpTemplate.Models;
using FluentValidation.AspNetCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;

namespace Appel.SharpTemplate.Tests
{
    public abstract class DependencyInjectionTest : IDisposable
    {
        private readonly SqliteConnection _connection;

        protected readonly SharpTemplateContext SharpTemplateContext;
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

            _connection = new SqliteConnection(configuration.GetConnectionString("DefaultConnection"));
            _connection.Open();

            var contextOptionsBuilder = new DbContextOptionsBuilder<SharpTemplateContext>()
                    .UseSqlite(_connection)
                    .Options;

            SharpTemplateContext = new SharpTemplateContext(contextOptionsBuilder);
            SharpTemplateContext.Database.EnsureCreated();

            JsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public void Dispose() => _connection.Close();
    }
}
