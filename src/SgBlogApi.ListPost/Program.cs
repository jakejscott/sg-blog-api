using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using SgBlogApi.Core;

namespace SgBlogApi.ListPost;

public static class Program
{
    public static async Task Main()
    {
        var logger = SerilogConfiguration.ConfigureLogging();

        var ddb = new AmazonDynamoDBClient();
        var store = new DynamoDbStore(ddb);

        var endpoint = new Endpoint(logger, store);

        Func<APIGatewayProxyRequest, Task<APIGatewayProxyResponse>> handler = endpoint.ExecuteAsync;

        await LambdaBootstrapBuilder.Create(handler,
                new SourceGeneratorLambdaJsonSerializer<SerializerContext>(options =>
                {
                    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                }))
            .Build()
            .RunAsync();
    }
}