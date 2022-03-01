using Appel.SharpTemplate.API.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls($"http://*:{Environment.GetEnvironmentVariable("PORT")}");
builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());
builder.WebHost.UseIISIntegration();

builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddServicesConfiguration();
builder.Services.AddSwaggerConfiguration();
builder.Services.AddGraphQLConfiguration();
builder.Services.AddCacheConfiguration();

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

app.UseApiConfiguration();
app.UseSwaggerConfiguration();
app.RunMigrations();

await app.RunAsync();