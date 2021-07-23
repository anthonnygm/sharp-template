﻿using Appel.SharpTemplate.Infrastructure;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Options;
using System;
using System.Text;

namespace Appel.SharpTemplate.Utils
{
    public class Argon2HashManager
    {
        private readonly IOptions<AppSettings> _appSettings;

        public Argon2HashManager(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        public string CreatePasswordHash(string value)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(value))
            {
                Salt = Encoding.ASCII.GetBytes(_appSettings.Value.Argon2PasswordKey),
                Iterations = 4,
                DegreeOfParallelism = 8,
                MemorySize = 1024 * 1024
            };

            return Convert.ToBase64String(argon2.GetBytes(16));
        }

        public bool VerifyPasswordHash(string password, string passwordHash)
        {
            var testPasswordHash = CreatePasswordHash(password);

            return passwordHash == testPasswordHash;
        }
    }
}