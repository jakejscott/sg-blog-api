using SgBlogApi.Core;

namespace SgBlogApi.IntegrationTests;

public class Given
{
    public CreatePostRequest CreatePostRequest()
    {
        return new CreatePostRequest
        {
            Body = "Body",
            Title = "Title"
        };
    }
    
    public UpdatePostRequest UpdatePostRequest()
    {
        return new UpdatePostRequest
        {
            Body = "Update Body",
            Title = "Update Title"
        };
    }
}