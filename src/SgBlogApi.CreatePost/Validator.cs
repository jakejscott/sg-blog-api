using FluentValidation;
using SgBlogApi.Core;

namespace SgBlogApi.CreatePost;

public class Validator : AbstractValidator<CreatePostRequest>
{
    public Validator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Body).NotEmpty();
    }
}