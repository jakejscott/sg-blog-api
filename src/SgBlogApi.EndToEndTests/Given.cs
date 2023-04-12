using SgBlogApi.Core;
using CreatePostRequest = SgBlogApi.Client.CreatePostRequest;
using UpdatePostRequest = SgBlogApi.Client.UpdatePostRequest;

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

    public UpdatePostRequest UpdatePostRequest()
    {
        var request = new UpdatePostRequest
        {
            Title = "Title-2",
            Body = "Body-2"
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