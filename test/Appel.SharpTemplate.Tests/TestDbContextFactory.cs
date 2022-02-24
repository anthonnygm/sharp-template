using Appel.SharpTemplate.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Appel.SharpTemplate.UnitTests;

public class TestDbContextFactory : IDbContextFactory<SharpTemplateContext>
{
    private readonly DbContextOptions<SharpTemplateContext> _options;

    public TestDbContextFactory()
    {
        _options = new DbContextOptionsBuilder<SharpTemplateContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    public SharpTemplateContext CreateDbContext()
    {
        return new SharpTemplateContext(_options);
    }
}
