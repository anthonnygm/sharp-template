using Xunit;

namespace Appel.SharpTemplate.Tests
{
    public class EnvironmentTests : DependencyInjectionTest
    {
        [Fact]
        public async void DatabaseIsAvailableAndCanBeConnectedTo()
        {
            Assert.True(await SharpTemplateContext.Database.CanConnectAsync());
        }
    }
}
