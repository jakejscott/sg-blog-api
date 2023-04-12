using FluentAssertions;

namespace SgBlogApi.EndToEndTests;

public class CreatePostTests
{
    [Fact]
    public async Task CreatePostOk()
    {
        var fixture = await Fixture.Ensure();
        var given = new Given();
        var client = fixture.Client;

        var now = DateTime.UtcNow;
        var blogId = Ulid.NewUlid().ToString();
        
        var request = given.CreatePostRequest();
        var response = await client.CreatePostAsync(blogId, request);
        
        response.Should().NotBeNull();
        response.Post.Should().NotBeNull();
        response.Post.PostId.Should().NotBeNullOrEmpty();
        response.Post.BlogId.Should().Be(blogId);
        response.Post.Title.Should().Be(request.Title);
        response.Post.Body.Should().Be(request.Body);
        
        // NOTE: Allow for some variation in the clock skew.
        response.Post.CreatedAt.Should().BeCloseTo(now, TimeSpan.FromMinutes(2));
        response.Post.UpdatedAt.Should().BeCloseTo(now, TimeSpan.FromMinutes(2));
    }
}