using System.Text.Json;
using Amazon.Lambda.APIGatewayEvents;
using FluentValidation;
using OneOf;
using SgBlogApi.Core;

namespace SgBlogApi.CreatePost;

public class CreatePostValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Body).NotEmpty();
    }
}

public class Endpoint
{
    private readonly CreatePostValidator _validator;
    private readonly DynamoDbStore _store;

    public Endpoint(DynamoDbStore store)
    {
        _validator = new CreatePostValidator();
        _store = store;
    }

    public async Task<APIGatewayProxyResponse> ExecuteAsync(APIGatewayProxyRequest apiRequest)
    {
        var request = JsonSerializer.Deserialize(apiRequest.Body, SerializerContext.Default.CreatePostRequest);

        var result = await CreatePostAsync(request);

        return result.Match(
            success => Response.From(success),
            invalidRequest => Response.From(invalidRequest),
            validationError => Response.From(validationError),
            serverError => Response.From(serverError));
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
            var result = await _store.CreatePostAsync(new ()
            {
                Title = request.Title!,
                Body = request.Body!,
            });

            return new CreatePostResponse
            {
                PostId = result.PostId
            };
        }
        catch (Exception ex)
        {
            return new ServerError(ex);
        }
    }
}