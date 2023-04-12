using FluentAssertions;

namespace SgBlogApi.EndToEndTests;

public class DeletePostTests
{
    [Fact]
    public async Task DeletePostOk()
    {
        var fixture = await Fixture.Ensure();
        var given = new Given();
        
        var client = fixture.Client;
        var store = fixture.Store;
        var mapper = fixture.Mapper;
        
        var blogId = Ulid.NewUlid().ToString();
        var args = given.CreatePostArgs(blogId);
        var entity  = await store.CreatePostAsync(args);

        var response = await client.DeletePostAsync(blogId, entity.PostId);
        response.Should().NotBeNull();
        
        var postDto = mapper.PostToDto(entity!);
        response!.Post.Should().BeEquivalentTo(postDto);
    }
}