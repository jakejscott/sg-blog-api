using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace SgBlogApi.Core;

public class CreatePostArgs
{
    public required string Title { get; set; }
    public required string Body { get; set; }
}

public class DynamoDbStore
{
    private readonly IAmazonDynamoDB _ddb;
    private readonly string _tableName;

    public DynamoDbStore(IAmazonDynamoDB ddb)
    {
        _tableName = Env.GetString("TABLE_NAME");
        _ddb = new AmazonDynamoDBClient();
    }

    public async Task<PostEntity> CreatePostAsync(CreatePostArgs args)
    {
        var postId = Ulid.NewUlid().ToString();
        var pk = $"POST#{postId}";
        var sk = $"POST#{postId}";
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

        var item = new Dictionary<string, AttributeValue>
        {
            { nameof(PostEntity.PK), new AttributeValue(pk) },
            { nameof(PostEntity.SK), new AttributeValue(sk) },
            { nameof(PostEntity.PostId), new AttributeValue(postId) },
            { nameof(PostEntity.Title), new AttributeValue(args.Title) },
            { nameof(PostEntity.Body), new AttributeValue(args.Body) },
            { nameof(PostEntity.CreatedAt), new AttributeValue(now.ToString("o")) },
            { nameof(PostEntity.UpdatedAt), new AttributeValue(now.ToString("o")) },
            { nameof(PostEntity.Entity), new AttributeValue("Post") }
        };

        await _ddb.PutItemAsync(new PutItemRequest
        {
            TableName = _tableName,
            Item = item
        });

        return entity;
    }
}