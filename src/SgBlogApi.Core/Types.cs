namespace SgBlogApi.Core;

public record InvalidRequest;
public record ValidationError(List<string> Errors);
public record ServerError;

public class ProblemDetailsResponse
{
    public int? StatusCode { get; set; }
    public string? ErrorCode { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class CreatePostRequest
{
    public string? Title { get; set; }
    public string? Body { get; set; }
}

public class CreatePostResponse
{
    public PostDto? Post { get; set; }
}