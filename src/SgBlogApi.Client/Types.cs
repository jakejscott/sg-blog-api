namespace SgBlogApi.Client;

public class CreatePostRequest
{
    public string? Title { get; set; }
    public string? Body { get; set; }
}

public class CreatePostResponse
{
    public required PostDto Post { get; init; }
}

public class GetPostResponse
{
    public required PostDto Post { get; init; }
}

public class UpdatePostRequest
{
    public required string Title { get; set; }
    public required string Body { get; set; }
}

public class UpdatePostResponse
{
    public required PostDto Post { get; init; }
}

public class DeletePostResponse
{
    public required PostDto Post { get; init; }
}

public class ListPostResponse
{
    public string? PaginationToken { get; init; }
    public required List<PostDto> Items { get; init; } = new();
}

public class PostDto
{
    public required string BlogId { get; init; }
    public required string PostId { get; init; }
    public required string Title { get; init; }
    public required string Body { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}