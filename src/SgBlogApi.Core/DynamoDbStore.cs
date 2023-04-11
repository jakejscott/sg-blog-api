namespace SgBlogApi.Core;

public class CreatePostArgs
{
    public required string Title { get; set; }
    public required string Body { get; set; }
}
    
public class CreatePostResult
{
    public required string PostId { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
}

public class DynamoDbStore
{
    public async Task<CreatePostResult> CreatePostAsync(CreatePostArgs args)
    {
        await Task.CompletedTask;
        
        var result = new CreatePostResult
        {
            PostId = Guid.NewGuid().ToString(),
            Title = args.Title,
            Body = args.Body,
        };

        return result;
    }
}