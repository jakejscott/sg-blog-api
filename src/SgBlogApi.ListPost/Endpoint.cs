using Amazon.Lambda.APIGatewayEvents;
using OneOf;
using Serilog;
using SgBlogApi.Core;

namespace SgBlogApi.ListPost;

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
        var result = await ListPostAsync(apiRequest);

        return result.Match(
            success => Response.From(success),
            invalid => Response.From(invalid),
            serverError => Response.From(serverError)
        );
    }

    private async Task<OneOf<ListPostResponse, InvalidRequest, ServerError>> ListPostAsync(APIGatewayProxyRequest apiRequest)
    {
        try
        {
            var blogId = apiRequest.PathParameters["blogId"]!;

            string? paginationToken = null;
            if (apiRequest.QueryStringParameters != null && apiRequest.QueryStringParameters.TryGetValue("paginationToken", out var rawToken))
            {
                paginationToken = rawToken;
            }

            int limit = 10;
            if (apiRequest.QueryStringParameters != null && apiRequest.QueryStringParameters.TryGetValue("limit", out var rawLimit))
            {
                if (int.TryParse(rawLimit, out var value))
                {
                    limit = value;
                }
            }
            
            var (items, nextPaginationToken) = await _store.ListPostAsync(blogId, limit, paginationToken);

            return new ListPostResponse
            {
                Items = items.Select(x => _mapper.PostToDto(x)).ToList(),
                PaginationToken = nextPaginationToken
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Something went wrong");
            return new ServerError();
        }
    }
}