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
        var postId = apiRequest.PathParameters["postId"];

        var result = await DeletePostAsync(postId);

        return result.Match(
            success => Response.From(success),
            invalidRequest => Response.From(invalidRequest),
            notFound => Response.From(notFound),
            serverError => Response.From(serverError)
        );
    }

    private async Task<OneOf<UpdatePostResponse, InvalidRequest, NotFound, ServerError>> DeletePostAsync(string? postId)
    {
        if (postId is null) return new InvalidRequest();

        try
        {
            var entity = await _store.DeletePostAsync(postId);

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