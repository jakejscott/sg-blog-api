using System.Text;
using Amazon;
using Amazon.APIGateway;
using Amazon.APIGateway.Model;
using Amazon.DynamoDBv2;
using Amazon.Runtime.CredentialManagement;
using dotenv.net;
using SgBlogApi.Client;
using SgBlogApi.Core;

namespace SgBlogApi.EndToEndTests;

public class Fixture
{
    public required DynamoDbStore Store { get; init; }
    public required ISgBlogClient Client { get; init; }
    
    public required PostMapper Mapper { get; init; }
    
    public static async Task<Fixture> Ensure() => await _factory;

    private static readonly AsyncLazy<Fixture> _factory = new(async () =>
    {
        DotEnv.Fluent()
            .WithTrimValues()
            .WithEncoding(Encoding.UTF8)
            .WithOverwriteExistingVars()
            .WithProbeForEnv(8)
            .Load();
        
        var region = Env.GetString("CDK_DEFAULT_REGION");
        var service = Env.GetString("SERVICE");
        var stage = Env.GetString("STAGE");
        
        Environment.SetEnvironmentVariable("TABLE_NAME", $"{service}-{stage}-app-blog");
        
        var chain = new CredentialProfileStoreChain();
        AmazonDynamoDBClient ddb;
        IAmazonAPIGateway apiGateway;
        
        if (chain.TryGetAWSCredentials("sg-dev", out var credentials))
        {
            // Running locally
            ddb = new AmazonDynamoDBClient(credentials, RegionEndpoint.GetBySystemName(region));
            apiGateway = new AmazonAPIGatewayClient(credentials, RegionEndpoint.GetBySystemName(region));
        }
        else
        {
            // Running in Github actions
            ddb = new AmazonDynamoDBClient();
            apiGateway = new AmazonAPIGatewayClient();
        }

        var store = new DynamoDbStore(ddb);
        var apiGatewayUrl = await apiGateway.GetApiGatewayUrlAsync(service, stage, region);
        var client = new SgBlogClient(new HttpClient(), new SgBlogClientConfig { ServiceUrl = apiGatewayUrl });

        return new Fixture
        {
            Store = store,
            Client = client,
            Mapper = new PostMapper()
        };
    });
}

public static class ApiGatewayExtensions
{
    public static async Task<Uri> GetApiGatewayUrlAsync(this IAmazonAPIGateway apiGateway, string service, string stage, string region)
    {
        var apis = await apiGateway.GetRestApisAsync(new GetRestApisRequest { Limit = 25 });
        var apiName = $"{service}-{stage}-app";
        var api = apis.Items.FirstOrDefault(x => x.Name.Equals(apiName, StringComparison.OrdinalIgnoreCase));
        
        if (api == null) throw new Exception($"Could not find Api Gateway {apiName}");
        
        var apiGatewayUrl = new Uri($"https://{api.Id}.execute-api.{region}.amazonaws.com/LIVE/");
        return apiGatewayUrl;
    }
}