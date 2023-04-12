using System.Text.Json;
using FluentAssertions;
using Serilog;
using SgBlogApi.Core;
using Xunit.Abstractions;

namespace SgBlogApi.IntegrationTests;

public class DeletePostTests
{
    private readonly ILogger _logger;

    public DeletePostTests(ITestOutputHelper output) => _logger = SerilogXUnitConfiguration.ConfigureLogging(output);

    [Fact]
    public async Task DeletePostOk()
    {
        var fixture = await Fixture.Ensure();
        var mapper = new PostMapper();
        var when = new When(_logger, fixture);
        var blogId = Ulid.NewUlid().ToString();

        var entity = await fixture.Store.CreatePostAsync(new CreatePostArgs
        {
            BlogId = blogId,
            Title = "Title",
            Body = "Body"
        });

        var dto = mapper.PostToDto(entity);

        var apiResponse = await when.DeletePostAsync(blogId, entity.PostId);
        apiResponse.StatusCode.Should().Be(200);
        
        var response = JsonSerializer.Deserialize<DeletePostResponse>(apiResponse.Body)!;
        response.Post.Should().NotBeNull();
        response.Post.Should().BeEquivalentTo(dto);
    }
    
    [Fact]
    public async Task DeletePostNotFound()
    {
        var fixture = await Fixture.Ensure();
        var when = new When(_logger, fixture);
        
        var blogId = Ulid.NewUlid().ToString();
        var postId = Ulid.NewUlid().ToString();
       
        var apiResponse = await when.DeletePostAsync(blogId, postId);
        apiResponse.StatusCode.Should().Be(404);
        
        var response = JsonSerializer.Deserialize<ProblemDetailsResponse>(apiResponse.Body)!;
        response.StatusCode.Should().Be(404);
        response.Errors.Should().HaveCount(0);
        response.ErrorCode.Should().Be(Response.ErrorCodes.NotFound);
    }
}