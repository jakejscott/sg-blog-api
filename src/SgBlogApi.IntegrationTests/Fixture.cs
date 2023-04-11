using System.Text;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime.CredentialManagement;
using dotenv.net;
using SgBlogApi.Core;

namespace SgBlogApi.IntegrationTests;

public class Fixture
{
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
        
        if (chain.TryGetAWSCredentials("sg-dev", out var credentials))
        {
            // Running locally
            ddb = new AmazonDynamoDBClient(credentials, RegionEndpoint.GetBySystemName(region));
        }
        else
        {
            // Running in Github actions
            ddb = new AmazonDynamoDBClient();
        }

        var store = new DynamoDbStore(ddb);

        await Task.CompletedTask;

        return new Fixture
        {
            Store = store
        };
    });

    public required DynamoDbStore Store { get; set; }
}