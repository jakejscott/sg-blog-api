using System.Text.Json;
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

public static class ErrorCodes
{
    public const string InvalidRequest = "INVALID_REQUEST";
    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
}


public static class Response
{
    public static APIGatewayProxyResponse From(CreatePostResponse response)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(response, SerializerContext.Default.CreatePostResponse)
        };
    }
    
    public static APIGatewayProxyResponse From(InvalidRequest invalidRequest)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 415,
            Body = JsonSerializer.Serialize(new ProblemDetailsResponse()
            {
                StatusCode = 415,
                ErrorCode = ErrorCodes.InvalidRequest,
                Errors = new()
            }, SerializerContext.Default.ProblemDetailsResponse)
        };
    }
    
    public static APIGatewayProxyResponse From(ValidationError validationError)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 400,
            Body = JsonSerializer.Serialize(new()
            {
                StatusCode = 400,
                ErrorCode = ErrorCodes.ValidationFailed,
                Errors = validationError.Errors
            }, SerializerContext.Default.ProblemDetailsResponse)
        };
    }
    
    public static APIGatewayProxyResponse From(ServerError serverError)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 500,
            Body = JsonSerializer.Serialize(new()
            {
                StatusCode = 500,
                ErrorCode = ErrorCodes.InternalServerError,
            }, SerializerContext.Default.ProblemDetailsResponse)
        };
    }
}