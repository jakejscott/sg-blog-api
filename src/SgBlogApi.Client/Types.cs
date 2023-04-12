namespace SgBlogApi.Client;

public record InvalidRequest;
public record NotFound;
public record ValidationError(List<string> Errors);
public record ServerError;

public class ProblemDetailsResponse
{
    public int? StatusCode { get; set; }
    public string? ErrorCode { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class PaginationToken
{
    public string? Pk { get; set; }
    public string? Sk { get; set; }
}

public class CreatePostRequest
{
    public string? Title { get; set; }
    public string? Body { get; set; }
}

public class CreatePostResponse
{
    public required PostDto Post { get; set; }
}

public class GetPostResponse
{
    public required PostDto Post { get; set; }
}

public class UpdatePostRequest
{
    public required string Title { get; set; }
    public required string Body { get; set; }
}

public class UpdatePostResponse
{
    public required PostDto Post { get; set; }
}

public class DeletePostResponse
{
    public required PostDto Post { get; set; }
}

public class ListPostResponse
{
    public string? PaginationToken { get; set; }
    public required List<PostDto> Items { get; set; } = new();
}

public class PostDto
{
    public required string BlogId { get; set; }
    public required string PostId { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}