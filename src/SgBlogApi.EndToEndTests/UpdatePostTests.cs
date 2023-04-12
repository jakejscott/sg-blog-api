using FluentAssertions;

namespace SgBlogApi.EndToEndTests;

public class UpdatePostTests
{
    [Fact]
    public async Task UpdatePostOk()
    {
        var fixture = await Fixture.Ensure();
        var given = new Given();
        
        var client = fixture.Client;
        var store = fixture.Store;

        var blogId = Ulid.NewUlid().ToString();
        var args = given.CreatePostArgs(blogId);
        var entity  = await store.CreatePostAsync(args);

        var now = DateTime.UtcNow;
        var request = given.UpdatePostRequest();
        var response = await client.UpdatePostAsync(blogId, entity.PostId, request);
        
        response.Should().NotBeNull();
        response.Post.Should().NotBeNull();
        response.Post.PostId.Should().Be(entity.PostId);
        response.Post.BlogId.Should().Be(blogId);
        response.Post.Title.Should().Be(request.Title);
        response.Post.Body.Should().Be(request.Body);
        response.Post.CreatedAt.Should().BeBefore(response.Post.UpdatedAt);
        
        // NOTE: Allow for some variation in the clock skew.
        response.Post.UpdatedAt.Should().BeCloseTo(now, TimeSpan.FromMinutes(2));
    }
}