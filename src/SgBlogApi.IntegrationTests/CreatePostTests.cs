using FluentAssertions;
using Serilog;
using Xunit.Abstractions;

namespace SgBlogApi.IntegrationTests;

public class CreatePostTests
{
    private readonly ILogger _logger;

    public CreatePostTests(ITestOutputHelper output) => _logger = SerilogXUnitConfiguration.ConfigureLogging(output);

    [Fact]
    public async Task CreatePostShouldSucceed()
    {
        var fixture = await Fixture.Ensure();
        var given = new Given();
        var when = new When(_logger, fixture);
        var blogId = "my-blog";
        var request = given.CreatePostRequest();
        var apiResponse = await when.CreatePostAsync(blogId, request);
        apiResponse.StatusCode.Should().Be(200);
    }
}