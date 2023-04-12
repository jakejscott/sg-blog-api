using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using Serilog;
using SgBlogApi.Core;

namespace SgBlogApi.IntegrationTests;

public class When
{
    private readonly ILogger _logger;
    private readonly Fixture _fixture;

    public When(ILogger logger, Fixture fixture)
    {
        _logger = logger;
        _fixture = fixture;
    }

    public async Task<APIGatewayProxyResponse> CreatePostAsync(string blogId, CreatePostRequest request)
    {
        var apiRequest = CreatePostRequest(request);
        apiRequest.PathParameters.Add("blogId", blogId);
        var endpoint = new CreatePost.Endpoint(_logger, _fixture.Store);
        return await endpoint.ExecuteAsync(apiRequest);
    }
    
    public async Task<APIGatewayProxyResponse> GetPostAsync(string blogId, string postId)
    {
        var apiRequest = CreateGetRequest();
        apiRequest.PathParameters.Add("blogId", blogId);
        apiRequest.PathParameters.Add("postId", postId);
        var endpoint = new GetPost.Endpoint(_logger, _fixture.Store);
        return await endpoint.ExecuteAsync(apiRequest);
    }
    
    public async Task<APIGatewayProxyResponse> DeletePostAsync(string blogId, string postId)
    {
        var apiRequest = CreateDeleteRequest();
        apiRequest.PathParameters.Add("blogId", blogId);
        apiRequest.PathParameters.Add("postId", postId);
        var endpoint = new DeletePost.Endpoint(_logger, _fixture.Store);
        return await endpoint.ExecuteAsync(apiRequest);
    }
    
    public async Task<APIGatewayProxyResponse> ListPostAsync(string blogId, int? limit, string? paginationToken)
    {
        var apiRequest = CreateGetRequest();
        apiRequest.PathParameters.Add("blogId", blogId);
        if (limit is not null) apiRequest.QueryStringParameters.Add("limit", limit.ToString());
        if (paginationToken is not null) apiRequest.QueryStringParameters.Add("paginationToken", paginationToken);
        var endpoint = new ListPost.Endpoint(_logger, _fixture.Store);
        return await endpoint.ExecuteAsync(apiRequest);
    }
    
    public async Task<APIGatewayProxyResponse> UpdatePostAsync(string blogId, string postId, UpdatePostRequest request)
    {
        var apiRequest = CreatePostRequest(request);
        apiRequest.PathParameters.Add("blogId", blogId);
        apiRequest.PathParameters.Add("postId", postId);
        var endpoint = new UpdatePost.Endpoint(_logger, _fixture.Store);
        return await endpoint.ExecuteAsync(apiRequest);
    }
    
    private APIGatewayProxyRequest CreatePostRequest<T>(T value)
    {
        var apiRequest = _CreateRequest("POST");
        apiRequest.Body = JsonSerializer.Serialize(value);
        return apiRequest;
    }

    private APIGatewayProxyRequest CreatePutRequest<T>(T value)
    {
        var apiRequest = _CreateRequest("PUT");
        apiRequest.Body = JsonSerializer.Serialize(value);
        return apiRequest;
    }

    private APIGatewayProxyRequest CreateDeleteRequest()
    {
        var apiRequest = _CreateRequest("DELETE");
        return apiRequest;
    }

    private APIGatewayProxyRequest CreateGetRequest()
    {
        var apiRequest = _CreateRequest("GET");
        return apiRequest;
    }

    private APIGatewayProxyRequest _CreateRequest(string httpMethod, string? roles = null, string? tenantId = null)
    {
        var apiRequest = new APIGatewayProxyRequest
        {
            HttpMethod = httpMethod,
            RequestContext = new APIGatewayProxyRequest.ProxyRequestContext
            {
                Authorizer = new APIGatewayCustomAuthorizerContext()
            },
            PathParameters = new Dictionary<string, string>(),
            QueryStringParameters = new Dictionary<string, string>()
        };

        if (roles != null) apiRequest.RequestContext.Authorizer.Add("roles", roles);
        if (tenantId != null) apiRequest.RequestContext.Authorizer.Add("tenantId", tenantId);

        return apiRequest;
    }
}