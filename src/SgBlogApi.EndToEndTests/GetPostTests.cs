using FluentAssertions;

namespace SgBlogApi.EndToEndTests;

public class GetPostTests
{
    [Fact]
    public async Task GetPostOk()
    {
        var fixture = await Fixture.Ensure();
        var given = new Given();
        
        var client = fixture.Client;
        var store = fixture.Store;
        var mapper = fixture.Mapper;
        
        var blogId = Ulid.NewUlid().ToString();
        var args = given.CreatePostArgs(blogId);
        var entity  = await store.CreatePostAsync(args);

        var response = await client.GetPostAsync(blogId, entity.PostId);
        
        response.Should().NotBeNull();
        
        var postDto = mapper.PostToDto(entity!);
        response!.Post.Should().BeEquivalentTo(postDto);
    }
    
    [Fact]
    public async Task GetPostNotFound()
    {
        var fixture = await Fixture.Ensure();
        var client = fixture.Client;
        
        var blogId = Ulid.NewUlid().ToString();
        var postId = Ulid.NewUlid().ToString();

        var response = await client.GetPostAsync(blogId, postId);
        response.Should().BeNull();
    }
}