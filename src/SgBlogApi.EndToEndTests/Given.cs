using SgBlogApi.Core;
using CreatePostRequest = SgBlogApi.Client.CreatePostRequest;

namespace SgBlogApi.EndToEndTests;

public class Given
{
    public CreatePostRequest CreatePostRequest()
    {
        var request = new CreatePostRequest
        {
            Title = "Title",
            Body = "Body"
        };

        return request;
    }

    public CreatePostArgs CreatePostArgs(string blogId)
    {
        var args = new CreatePostArgs
        {
            BlogId = blogId,
            Title = "Title",
            Body = "Body"
        };

        return args;
    }
}