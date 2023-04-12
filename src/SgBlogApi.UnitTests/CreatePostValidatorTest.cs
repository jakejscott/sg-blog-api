using FluentValidation.TestHelper;
using SgBlogApi.Core;

namespace SgBlogApi.UnitTests;

public class CreatePostValidatorTest
{
    readonly CreatePost.Validator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TitleIsRequired(string value)
    {
        var result = _validator.TestValidate(new CreatePostRequest { Title = value });
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void BodyIsRequired(string value)
    {
        var result = _validator.TestValidate(new CreatePostRequest { Body = value });
        result.ShouldHaveValidationErrorFor(x => x.Body);
    }
}