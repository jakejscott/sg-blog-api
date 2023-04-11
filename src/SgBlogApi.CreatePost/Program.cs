using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using SgBlogApi.Core;

namespace SgBlogApi.CreatePost;

public static class Program
{
    private static Endpoint? _endpoint;

    public static async Task Main()
    {
        SourceGeneratorLambdaJsonSerializer<SerializerContext> serializer = new (x =>
        {
            x.PropertyNameCaseInsensitive = true;
        });

        var store = new DynamoDbStore();
        
        _endpoint = new Endpoint(store);
        
        Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>> handler = _endpoint.ExecuteAsync;

        await LambdaBootstrapBuilder.Create(handler, serializer).Build().RunAsync();
    }
}