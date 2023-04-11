using FluentAssertions;
using SgBlogApi.Core;

namespace SgBlogApi.UnitTests;

public class PostEntityTests
{
    [Fact]
    public void RoundTrip()
    {
        var postId = Ulid.NewUlid().ToString();
        var pk = $"POST#{postId}";
        var sk = $"POST#{postId}";
        var now = DateTime.UtcNow;

        var entity = new PostEntity
        {
            Pk = pk,
            Sk = sk,
            Entity = "Post",
            PostId = postId,
            Title = "Title",
            Body = "Body",
            CreatedAt = now,
            UpdatedAt = now,
        };

        var item = entity.ToItem();
        PostEntity.FromItem(item).Should().BeEquivalentTo(entity);
    }
}