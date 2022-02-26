using Appel.SharpTemplate.Infrastructure.Application;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;
using System;
using System.Text;

namespace Appel.SharpTemplate.API.Application.Extensions;

public class Argon2HashManager
{
    private readonly IOptionsMonitor<AppSettings> _appSettings;

    public Argon2HashManager(IOptionsMonitor<AppSettings> appSettings)
    {
        _appSettings = appSettings;
    }

    public string CreatePasswordHash(string value)
    {
        var argon2 = new Argon2id(Encoding.UTF8.GetBytes(value))
        {
            Salt = Encoding.ASCII.GetBytes(_appSettings.CurrentValue.Argon2PasswordKey),
            Iterations = 2,
            DegreeOfParallelism = 2,
            MemorySize = 128 * 128
        };

        return Convert.ToBase64String(argon2.GetBytes(16));
    }

    public bool VerifyPasswordHash(string password, string passwordHash)
    {
        var testPasswordHash = CreatePasswordHash(password);

        return passwordHash == testPasswordHash;
    }
}
