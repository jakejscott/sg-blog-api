using Amazon.Runtime;
using EfficientDynamoDb;
using EfficientDynamoDb.Configs;
using EfficientDynamoDb.Credentials.AWSSDK;

namespace SgBlogApi.Core;

public class CreatePostArgs
{
    public required string Title { get; set; }
    public required string Body { get; set; }
}

public class DynamoDbStore
{
    private readonly DynamoDbContext _ddb;

    public DynamoDbStore(AWSCredentials credentials)
    {
        var service = Env.GetString("SERVICE");
        var stage = Env.GetString("STAGE");
        var region = Env.GetString("AWS_REGION");
        var regionEndpoint = RegionEndpoint.Create(region);
        var provider = new AWSCredentialsProvider(credentials);
        var tableNamePrefix = $"{service}-{stage}-app-";

        var config = new DynamoDbContextConfig(regionEndpoint, provider)
        {
            TableNamePrefix = tableNamePrefix
        };

        _ddb = new DynamoDbContext(config);
    }

    public async Task<PostEntity> CreatePostAsync(CreatePostArgs args)
    {
        var postId = Ulid.NewUlid().ToString();
        var now = DateTime.UtcNow;

        var entity = new PostEntity
        {
            PK = $"POST#{postId}",
            SK = $"POST#{postId}",
            PostId = postId,
            Title = args.Title,
            Body = args.Body,
            CreatedAt = now,
            UpdatedAt = now,
            Entity = "Post",
        };

        await _ddb.PutItemAsync(entity);

        return entity;
    }
}