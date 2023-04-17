using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using FluentValidation.Results;
using OneOf;
using Serilog;
using SgBlogApi.Core;

namespace SgBlogApi.CreatePost;

public class Endpoint
{
    private readonly Validator _validator;
    private readonly ILogger _logger;
    private readonly DynamoDbStore _store;
    private readonly SerializerContext _serializerContext;
    private readonly PostMapper _mapper;

    public Endpoint(ILogger logger, DynamoDbStore store)
    {
        _validator = new Validator();
        _serializerContext = new SerializerContext(new() { PropertyNameCaseInsensitive = true });
        _logger = logger;
        _store = store;
        _mapper = new PostMapper();
    }

    public async Task<APIGatewayProxyResponse> ExecuteAsync(APIGatewayProxyRequest apiRequest)
    {
        var result = await CreatePostAsync(apiRequest);

        return result.Match(
            success => Response.From(success),
            invalidRequest => Response.From(invalidRequest),
            validationError => Response.From(validationError),
            serverError => Response.From(serverError)
        );
    }

    private async Task<OneOf<CreatePostResponse, InvalidRequest, ValidationError, ServerError>> CreatePostAsync(APIGatewayProxyRequest apiRequest)
    {
        try
        {
            if (apiRequest.Body is null)
                return new InvalidRequest();

            CreatePostRequest? request = JsonSerializer.Deserialize(apiRequest.Body, _serializerContext.CreatePostRequest);
            if (request is null)
                return new InvalidRequest();

            ValidationResult? validation = await _validator.ValidateAsync(request);
            if (!validation.IsValid)
                return new ValidationError(validation);

            var blogId = apiRequest.PathParameters["blogId"]!;

            PostEntity entity = await _store.CreatePostAsync(new()
            {
                BlogId = blogId,
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
            _logger.Error(ex, "Something went wrong");
            return new ServerError();
        }
    }
}