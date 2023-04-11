using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Runtime;
using SgBlogApi.Core;

namespace SgBlogApi.CreatePost;

public static class Program
{
    public static async Task Main()
    {
        var credentials = new EnvironmentVariablesAWSCredentials();
        var store = new DynamoDbStore(credentials);
        var endpoint = new Endpoint(store);
        
        Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>> handler = endpoint.ExecuteAsync;
        
        await LambdaBootstrapBuilder.Create(handler, new SourceGeneratorLambdaJsonSerializer<SerializerContext>())
            .Build()
            .RunAsync();
    }
}