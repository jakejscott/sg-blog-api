using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Extensions;
using Serilog;
using SgBlogApi.Core;
using Xunit.Abstractions;

namespace SgBlogApi.IntegrationTests;

public class ListPostTests
{
    private readonly ILogger _logger;

    public ListPostTests(ITestOutputHelper output) => _logger = SerilogXUnitConfiguration.ConfigureLogging(output);

    [Fact]
    public async Task ListPostShouldSucceed()
    {
        var fixture = await Fixture.Ensure();
        var when = new When(_logger, fixture);
        var blogId = Ulid.NewUlid().ToString();
        var otherBlogId = Ulid.NewUlid().ToString();

        await fixture.Store.CreatePostAsync(new ()
        {
            BlogId = blogId,
            Body = "Body 1",
            Title = "Title 1",
        });
        
        await fixture.Store.CreatePostAsync(new ()
        {
            BlogId = blogId,
            Body = "Body 2",
            Title = "Title 2",
        });
        
        await fixture.Store.CreatePostAsync(new ()
        {
            BlogId = blogId,
            Body = "Body 3",
            Title = "Title 3",
        });
        
        await fixture.Store.CreatePostAsync(new ()
        {
            BlogId = otherBlogId,
            Body = "Other 3",
            Title = "Other 3",
        });

        Func<Task> asyncRetry = async () =>
        {
            {
                var apiResponse1 = await when.ListPostAsync(blogId, limit: 25, paginationToken: null);
                apiResponse1.StatusCode.Should().Be(200);

                var listPostResponse1 = JsonSerializer.Deserialize<ListPostResponse>(apiResponse1.Body)!;
                listPostResponse1.Items.Count.Should().Be(3);
                listPostResponse1.PaginationToken.Should().BeNullOrEmpty();
            }
            
            {
                var apiResponse1 = await when.ListPostAsync(otherBlogId, limit: 25, paginationToken: null);
                apiResponse1.StatusCode.Should().Be(200);

                var listPostResponse1 = JsonSerializer.Deserialize<ListPostResponse>(apiResponse1.Body)!;
                listPostResponse1.Items.Count.Should().Be(1);
                listPostResponse1.PaginationToken.Should().BeNullOrEmpty();
            }

            {
                var apiResponse1 = await when.ListPostAsync(blogId, limit: 2, paginationToken: null);
                apiResponse1.StatusCode.Should().Be(200);
                var listPostResponse1 = JsonSerializer.Deserialize<ListPostResponse>(apiResponse1.Body)!;
                listPostResponse1.Items.Count.Should().Be(2);
                listPostResponse1.PaginationToken.Should().NotBeNullOrEmpty();

                var apiResponse2 = await when.ListPostAsync(blogId, limit: 2, paginationToken: listPostResponse1.PaginationToken);
                apiResponse2.StatusCode.Should().Be(200);
                var listPostResponse2 = JsonSerializer.Deserialize<ListPostResponse>(apiResponse2.Body)!;
                listPostResponse2.Items.Count.Should().Be(1);
                listPostResponse2.PaginationToken.Should().BeNullOrEmpty();
            }
        };
        
        await asyncRetry.Should().NotThrowAfterAsync(waitTime: 5.Seconds(), pollInterval: 1.Seconds());
    }
}