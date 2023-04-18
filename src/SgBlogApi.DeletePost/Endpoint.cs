using Amazon.Lambda.APIGatewayEvents;
using OneOf;
using Serilog;
using SgBlogApi.Core;

namespace SgBlogApi.DeletePost;

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
        var result = await DeletePostAsync(apiRequest);

        return result.Match(
            success => Response.From(success),
            invalidRequest => Response.From(invalidRequest),
            notFound => Response.From(notFound),
            serverError => Response.From(serverError)
        );
    }

    private async Task<OneOf<UpdatePostResponse, InvalidRequest, NotFound, ServerError>> DeletePostAsync(APIGatewayProxyRequest apiRequest)
    {
        try
        {
            var blogId = apiRequest.PathParameters["blogId"]!;
            var postId = apiRequest.PathParameters["postId"]!;

            var entity = await _store.DeletePostAsync(blogId, postId);

            return entity switch
            {
                null => new NotFound(),
                _ => new UpdatePostResponse { Post = _mapper.PostToDto(entity) }
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Something went wrong");
            return new ServerError();
        }
    }
}