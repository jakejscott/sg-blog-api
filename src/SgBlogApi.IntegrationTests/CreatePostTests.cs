using System.Text.Json;
using FluentAssertions;
using Serilog;
using SgBlogApi.Core;
using Xunit.Abstractions;

namespace SgBlogApi.IntegrationTests;

public class CreatePostTests
{
    private readonly ILogger _logger;

    public CreatePostTests(ITestOutputHelper output) => _logger = SerilogXUnitConfiguration.ConfigureLogging(output);

    [Fact]
    public async Task CreatePostOk()
    {
        var mapper = new PostMapper();
        var fixture = await Fixture.Ensure();
        var given = new Given();
        var when = new When(_logger, fixture);
        var blogId = Ulid.NewUlid().ToString();
        
        var request = given.CreatePostRequest();
        var apiResponse = await when.CreatePostAsync(blogId, request);
        apiResponse.StatusCode.Should().Be(200);

        var response = JsonSerializer.Deserialize<CreatePostResponse>(apiResponse.Body)!;
        response.Post.Should().NotBeNull();

        var entity = await fixture.Store.GetPostAsync(blogId, response.Post!.PostId!);
        entity.Should().NotBeNull();
        
        var postDto = mapper.PostToDto(entity!);
        response.Post.Should().BeEquivalentTo(postDto);
    }
    
    [Fact]
    public async Task CreatePostValidation()
    {
        var fixture = await Fixture.Ensure();
        var when = new When(_logger, fixture);
        var blogId = Ulid.NewUlid().ToString();

        var request = new CreatePostRequest(); // NOTE: Missing Title and Body

        var apiResponse = await when.CreatePostAsync(blogId, request);
        apiResponse.StatusCode.Should().Be(400);

        var response = JsonSerializer.Deserialize<ProblemDetailsResponse>(apiResponse.Body)!;
        response.StatusCode.Should().Be(400);
        response.Errors.Should().HaveCount(2);
        response.ErrorCode.Should().Be(Response.ErrorCodes.ValidationFailed);
    }
}

