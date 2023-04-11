using Amazon.Lambda.APIGatewayEvents;
using OneOf;
using Serilog;
using SgBlogApi.Core;

namespace SgBlogApi.ReadPost;

public class Endpoint
{
    private readonly ILogger _logger;
    private readonly DynamoDbStore _store;
    private readonly PostMapper _mapper;

    public Endpoint(ILogger logger, DynamoDbStore store)
    {
        _logger = logger;
        _store = store;
        _mapper = new PostMapper();
    }

    public async Task<APIGatewayProxyResponse> ExecuteAsync(APIGatewayProxyRequest apiRequest)
    {
        var blogId = apiRequest.PathParameters["blogId"];
        var postId = apiRequest.PathParameters["postId"];
        
        var result = await GetPostAsync(blogId, postId);

        return result.Match(
            success => Response.From(success),
            invalid => Response.From(invalid),
            notFound => Response.From(notFound),
            serverError => Response.From(serverError)
        );
    }

    private async Task<OneOf<GetPostResponse, InvalidRequest, NotFound, ServerError>> GetPostAsync(string? blogId, string? postId)
    {
        if (blogId is null || postId is null) return new InvalidRequest();
        
        try
        {
            PostEntity? entity = await _store.GetPostAsync(blogId, postId);
            
            return entity switch
            {
                null => new NotFound(),
                _ => new GetPostResponse { Post = _mapper.PostToDto(entity) }
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Something went wrong");
            return new ServerError();
        }
    }
}