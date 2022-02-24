using Appel.SharpTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Appel.SharpTemplate.UnitTests;

public class TestDbContextFactory : IDbContextFactory<SharpTemplateContext>
{
    private readonly DbContextOptions<SharpTemplateContext> _options;

    public TestDbContextFactory(string databaseName = "test-db")
    {
        _options = new DbContextOptionsBuilder<SharpTemplateContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
    }

    public SharpTemplateContext CreateDbContext()
    {
        return new SharpTemplateContext(_options);
    }
}
