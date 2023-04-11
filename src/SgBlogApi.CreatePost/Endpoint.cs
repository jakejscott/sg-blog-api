﻿using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using OneOf;
using SgBlogApi.Core;

namespace SgBlogApi.CreatePost;

public class Endpoint
{
    private readonly Validator _validator;
    private readonly DynamoDbStore _store;
    private readonly SerializerContext _serializerContext;
    private readonly PostMapper _mapper;

    public Endpoint(DynamoDbStore store)
    {
        _validator = new Validator();
        _serializerContext = new SerializerContext(new () { PropertyNameCaseInsensitive = true });
        _store = store;
        _mapper = new PostMapper();
    }

    public async Task<APIGatewayProxyResponse> ExecuteAsync(APIGatewayProxyRequest apiRequest)
    {
        CreatePostRequest? request = JsonSerializer.Deserialize(apiRequest.Body, _serializerContext.CreatePostRequest);

        var result = await CreatePostAsync(request);

        APIGatewayProxyResponse? response = result.Match(
            success => Response.From(success),
            invalidRequest => Response.From(invalidRequest),
            validationError => Response.From(validationError),
            serverError => Response.From(serverError)
        );

        return response;
    }

    private async Task<OneOf<CreatePostResponse, InvalidRequest, ValidationError, ServerError>> CreatePostAsync(CreatePostRequest? request)
    {
        if (request == null) return new InvalidRequest();
        
        var validation = await _validator.ValidateAsync(request);
        
        if (!validation.IsValid)
        {
            return new ValidationError(validation.Errors.Select(x => x.ErrorMessage).ToList());
        }

        try
        {
            PostEntity entity = await _store.CreatePostAsync(new CreatePostArgs
            {
                Title = request.Title!,
                Body = request.Body!,
            });

            PostDto dto = _mapper.PostToDto(entity);

            return new CreatePostResponse
            {
                Post = dto
            };
        }
        catch (Exception ex)
        {
            return new ServerError(ex);
        }
    }
}