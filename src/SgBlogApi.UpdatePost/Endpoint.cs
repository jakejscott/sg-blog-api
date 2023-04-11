using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using OneOf;
using Serilog;
using SgBlogApi.Core;

namespace SgBlogApi.ReadPost;

public class Endpoint
{
    private readonly ILogger _logger;
    private readonly DynamoDbStore _store;
    private readonly SerializerContext _serializerContext;
    private readonly PostMapper _mapper;
    private readonly Validator _validator;

    public Endpoint(ILogger logger, DynamoDbStore store)
    {
        _logger = logger;
        _store = store;
        _mapper = new PostMapper();
        _validator = new Validator();
        _serializerContext = new SerializerContext(new() { PropertyNameCaseInsensitive = true });
    }

    public async Task<APIGatewayProxyResponse> ExecuteAsync(APIGatewayProxyRequest apiRequest)
    {
        var blogId = apiRequest.PathParameters["blogId"];
        var postId = apiRequest.PathParameters["postId"];
        
        UpdatePostRequest? request = JsonSerializer.Deserialize(apiRequest.Body, _serializerContext.UpdatePostRequest);

        var result = await UpdatePostAsync(blogId, postId, request);

        return result.Match(
            success => Response.From(success),
            invalidRequest => Response.From(invalidRequest),
            validationError => Response.From(validationError),
            notFound => Response.From(notFound),
            serverError => Response.From(serverError)
        );
    }

    private async Task<OneOf<UpdatePostResponse, InvalidRequest, ValidationError, NotFound, ServerError>> UpdatePostAsync(
        string? blogId, string? postId, UpdatePostRequest? request)
    {
        if (blogId is null || postId is null || request is null) return new InvalidRequest();
        
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            return new ValidationError(validation.Errors.Select(x => x.ErrorMessage).ToList());
        }

        try
        {
            var entity = await _store.UpdatePostAsync(new ()
            {
                BlogId = blogId,
                PostId = postId,
                Title = request.Title!,
                Body = request.Body!,
            });

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