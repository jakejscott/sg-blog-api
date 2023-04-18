using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using SgBlogApi.Core;
using SgBlogApi.UpdatePost;

var logger = SerilogConfiguration.ConfigureLogging();
var ddb = new AmazonDynamoDBClient();
var store = new DynamoDbStore(ddb);

var endpoint = new Endpoint(logger, store);
var handler = endpoint.ExecuteAsync;

var serializer = new SourceGeneratorLambdaJsonSerializer<SerializerContext>(
    x => x.PropertyNamingPolicy = JsonNamingPolicy.CamelCase
);

await LambdaBootstrapBuilder.Create(handler, serializer).Build().RunAsync();