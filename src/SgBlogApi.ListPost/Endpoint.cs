﻿using Amazon.Lambda.APIGatewayEvents;
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

        string? paginationToken = null;
        if (apiRequest.QueryStringParameters.TryGetValue("paginationToken", out var rawToken))
        {
            paginationToken = rawToken;
        }

        int? limit = null;
        if (apiRequest.QueryStringParameters.TryGetValue("limit", out var rawLimit) && int.TryParse(rawLimit, out var value))
        {
            limit = value;
        }

        var result = await ListPostAsync(blogId, limit ?? 10, paginationToken);

        return result.Match(
            success => Response.From(success),
            invalid => Response.From(invalid),
            notFound => Response.From(notFound),
            serverError => Response.From(serverError)
        );
    }

    private async Task<OneOf<ListPostResponse, InvalidRequest, NotFound, ServerError>> ListPostAsync(
        string? blogId, int limit, string? paginationToken)
    {
        if (blogId is null) return new InvalidRequest();

        try
        {
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