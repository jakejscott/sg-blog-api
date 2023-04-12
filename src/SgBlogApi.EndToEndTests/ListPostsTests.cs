using FluentAssertions;
using FluentAssertions.Extensions;
using SgBlogApi.Core;

namespace SgBlogApi.EndToEndTests;

public class ListPostsTests
{
    [Fact]
    public async Task ListPostsOk()
    {
        var fixture = await Fixture.Ensure();
        var given = new Given();
        
        var client = fixture.Client;
        var store = fixture.Store;
        var mapper = fixture.Mapper;

        var blogId = Ulid.NewUlid().ToString();
        
        int total = 10;
        var limit = 4;
        var page = 0;
        string? paginationToken = null;

        var entities = new List<PostEntity>();
        
        {
            for (var i = 0; i < total; i++)
            {
                var args = given.CreatePostArgs(blogId);
                var entity = await store.CreatePostAsync(args);

                entities.Add(entity);
            }
        }

        {
            
            var args = given.CreatePostArgs(Ulid.NewUlid().ToString()); // NOTE: Different blog id
            await store.CreatePostAsync(args);
        }

        // NOTE: Dynamo returns the newest posts first (ScanIndexForward = false)
        // We need to reverse the list of entities so that the posts are sorted in descending order.
        entities.Reverse();

        Func<Task> asyncRetry = async () =>
        {
            do
            {
                var response = await client.ListPostsAsync(blogId, limit, paginationToken);
                response.Should().NotBeNull();

                if (response.PaginationToken != null)
                {
                    response.Items.Count.Should().Be(limit);
                }
                else
                {
                    response.Items.Count.Should().BeLessThan(limit);
                }

                response.Items.Should().OnlyHaveUniqueItems();

                foreach (var entity in entities.Skip(page * limit).Take(limit).ToList())
                {
                    var postDto = mapper.PostToDto(entity);
                    response.Items.Should().ContainEquivalentOf(postDto);
                }

                paginationToken = response.PaginationToken;
                page++;

            } while (paginationToken != null);
        };
        
        await asyncRetry.Should().NotThrowAfterAsync(waitTime: 5.Seconds(), pollInterval: 1.Seconds());

        page.Should().Be(3);
    }
}