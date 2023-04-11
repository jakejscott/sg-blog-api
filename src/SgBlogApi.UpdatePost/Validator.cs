using FluentValidation;
using SgBlogApi.Core;

namespace SgBlogApi.ReadPost;

public class Validator : AbstractValidator<UpdatePostRequest>
{
    public Validator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Body).NotEmpty();
    }
}