using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;

namespace SgBlogApi.Core;

public static class Response
{
    private static class ErrorCodes
    {
        public const string InvalidRequest = "INVALID_REQUEST";
        public const string ValidationFailed = "VALIDATION_FAILED";
        public const string InternalServerError = "INTERNAL_SERVER_ERROR";
        public const string NotFound = "NOT_FOUND";
    }
    
    public static APIGatewayProxyResponse From(CreatePostResponse response)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(response, SerializerContext.Default.CreatePostResponse)
        };
    }
    
    public static APIGatewayProxyResponse From(GetPostResponse response)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(response, SerializerContext.Default.GetPostResponse)
        };
    }
    
    public static APIGatewayProxyResponse From(UpdatePostResponse response)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(response, SerializerContext.Default.UpdatePostResponse)
        };
    }
    
    public static APIGatewayProxyResponse From(InvalidRequest invalidRequest)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 415,
            Body = JsonSerializer.Serialize(new ProblemDetailsResponse
            {
                StatusCode = 415,
                ErrorCode = ErrorCodes.InvalidRequest,
                Errors = new()
            }, SerializerContext.Default.ProblemDetailsResponse)
        };
    }
    
    public static APIGatewayProxyResponse From(NotFound notFound)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 404,
            Body = JsonSerializer.Serialize(new ProblemDetailsResponse
            {
                StatusCode = 404,
                ErrorCode = ErrorCodes.NotFound,
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