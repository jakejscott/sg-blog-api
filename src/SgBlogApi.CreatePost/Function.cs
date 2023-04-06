using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using SgBlogApi.Core;

namespace SgBlogApi.CreatePost;

public class Function
{
    public static async Task Main()
    {
        var serializer = new SourceGeneratorLambdaJsonSerializer<CustomJsonSerializerContext>(options =>
        {
            options.PropertyNameCaseInsensitive = true;
        });

        var builder = LambdaBootstrapBuilder.Create((APIGatewayProxyRequest request, ILambdaContext context) =>
        {
            var response = new APIGatewayProxyResponse
            {
                Body = "Hello",
                StatusCode = 200,
            };

            return Task.FromResult(response);
        }, serializer);

        var bootstrap = builder.Build();
        await bootstrap.RunAsync();
    }
}