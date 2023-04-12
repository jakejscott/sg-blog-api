using FluentValidation.TestHelper;
using SgBlogApi.Core;

namespace SgBlogApi.UnitTests;

public class UpdatePostValidatorTest
{
    readonly UpdatePost.Validator _validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TitleIsRequired(string value)
    {
        var result = _validator.TestValidate(new UpdatePostRequest { Title = value });
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void BodyIsRequired(string value)
    {
        var result = _validator.TestValidate(new UpdatePostRequest { Body = value });
        result.ShouldHaveValidationErrorFor(x => x.Body);
    }
}