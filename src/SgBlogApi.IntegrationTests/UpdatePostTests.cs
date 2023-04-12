using System.Text.Json;
using FluentAssertions;
using Serilog;
using SgBlogApi.Core;
using Xunit.Abstractions;

namespace SgBlogApi.IntegrationTests;

public class UpdatePostTests
{
    private readonly ILogger _logger;

    public UpdatePostTests(ITestOutputHelper output) => _logger = SerilogXUnitConfiguration.ConfigureLogging(output);

    [Fact]
    public async Task UpdatePostOk()
    {
        var fixture = await Fixture.Ensure();
        var given = new Given();
        var when = new When(_logger, fixture);
        var blogId = Ulid.NewUlid().ToString();

        var entity = await fixture.Store.CreatePostAsync(new CreatePostArgs
        {
            BlogId = blogId,
            Title = "Title",
            Body = "Body"
        });

        var request = given.UpdatePostRequest();
        var apiResponse = await when.UpdatePostAsync(blogId, entity.PostId, request);
        apiResponse.StatusCode.Should().Be(200);
        
        var response = JsonSerializer.Deserialize<UpdatePostResponse>(apiResponse.Body)!;
        response.Post.Should().NotBeNull();
        response.Post!.Title.Should().Be(request.Title);
        response.Post!.Body.Should().Be(request.Body);
    }
    
    [Fact]
    public async Task UpdatePostValidation()
    {
        var fixture = await Fixture.Ensure();
        var when = new When(_logger, fixture);
        var blogId = Ulid.NewUlid().ToString();

        var entity = await fixture.Store.CreatePostAsync(new CreatePostArgs
        {
            BlogId = blogId,
            Title = "Title",
            Body = "Body"
        });

        var request = new UpdatePostRequest(); // NOTE: Missing Title and Body

        var apiResponse = await when.UpdatePostAsync(blogId, entity.PostId, request);
        apiResponse.StatusCode.Should().Be(400);
        
        var response = JsonSerializer.Deserialize<ProblemDetailsResponse>(apiResponse.Body)!;
        response.StatusCode.Should().Be(400);
        response.Errors.Should().HaveCount(2);
        response.ErrorCode.Should().Be(Response.ErrorCodes.ValidationFailed);
    }
    
    [Fact]
    public async Task CreatePostNotFound()
    {
        var fixture = await Fixture.Ensure();
        var given = new Given();
        var when = new When(_logger, fixture);
        var blogId = Ulid.NewUlid().ToString();
        var postId = Ulid.NewUlid().ToString();

        var request = given.UpdatePostRequest();
        var apiResponse = await when.UpdatePostAsync(blogId, postId, request);
        apiResponse.StatusCode.Should().Be(404);
    
        var response = JsonSerializer.Deserialize<ProblemDetailsResponse>(apiResponse.Body)!;
        response.StatusCode.Should().Be(404);
        response.Errors.Should().HaveCount(0);
        response.ErrorCode.Should().Be(Response.ErrorCodes.NotFound);
    }
}