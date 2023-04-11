using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;

namespace SgBlogApi.Core;

public record InvalidRequest;
public record ValidationError(List<string> Errors);
public record ServerError(Exception Exception);

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
    public string? PostId { get; set; }
}

[JsonSerializable(typeof(CreatePostRequest))]
[JsonSerializable(typeof(CreatePostResponse))]
[JsonSerializable(typeof(APIGatewayProxyRequest))]
[JsonSerializable(typeof(APIGatewayProxyResponse))]
[JsonSerializable(typeof(ProblemDetailsResponse))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
public partial class SerializerContext : JsonSerializerContext
{
}