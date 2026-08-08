using System.Net;
using FluentAssertions;

namespace TuttiWallet.Api.IntegrationTests;

public class HealthCheckTests(ApiWebApplicationFactory factory) : IClassFixture<ApiWebApplicationFactory>
{
    [Fact]
    public async Task Health_WithDatabaseReachable_ReturnsOk()
    {
        var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
