using System.Text.Json;
using FluentAssertions;
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
        var blogId = "my-blog";

        // {
        //     var apiResponse = await when.ListPostAsync(blogId, limit: null, paginationToken: null);
        //     apiResponse.StatusCode.Should().Be(200);
        // }
        //
        // {
        //     var apiResponse = await when.ListPostAsync(blogId, limit: 10, paginationToken: null);
        //     apiResponse.StatusCode.Should().Be(200);
        // }
        
        
        {
            var apiResponse1 = await when.ListPostAsync(blogId, limit: 1, paginationToken: null);
            apiResponse1.StatusCode.Should().Be(200);

            var listPostResponse1 = JsonSerializer.Deserialize<ListPostResponse>(apiResponse1.Body)!;
            listPostResponse1.Items.Count.Should().Be(1);
            listPostResponse1.PaginationToken.Should().NotBeEmpty();
            
            var apiResponse2 = await when.ListPostAsync(blogId, limit: 1, paginationToken: listPostResponse1.PaginationToken);
            apiResponse2.StatusCode.Should().Be(200);
            
            var listPostResponse2 = JsonSerializer.Deserialize<ListPostResponse>(apiResponse2.Body)!;
            listPostResponse2.Items.Count.Should().Be(1);
            // listPostResponse2.PaginationToken.Should().BeNullOrEmpty();
            
            var apiResponse3 = await when.ListPostAsync(blogId, limit: 1, paginationToken: listPostResponse2.PaginationToken);
            apiResponse3.StatusCode.Should().Be(200);
            var listPostResponse3 = JsonSerializer.Deserialize<ListPostResponse>(apiResponse3.Body)!;
            listPostResponse3.PaginationToken.Should().BeNullOrEmpty();


        }
    }
}