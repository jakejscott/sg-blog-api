using FluentAssertions;
using SgBlogApi.Core;

namespace SgBlogApi.UnitTests;

public class PostEntityTests
{
    [Fact]
    public void RoundTrip()
    {
        var entity = PostEntity.Create(new()
        {
            BlogId = "my-blog",
            Body = "Body", 
            Title = "Title"
        });
        var item = entity.ToItem();
        var test = PostEntity.FromItem(item);
        test.Should().BeEquivalentTo(entity);
    }
}