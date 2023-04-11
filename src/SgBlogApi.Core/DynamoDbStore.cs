using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace SgBlogApi.Core;

public class DynamoDbStore
{
    private readonly IAmazonDynamoDB _ddb;
    private readonly string _tableName;

    public class CreatePostArgs
    {
        public required string Title { get; init; }
        public required string Body { get; init; }
    }

    public class UpdatePostArgs
    {
        public required string Title { get; init; }
        public required string Body { get; init; }
    }

    public DynamoDbStore(IAmazonDynamoDB ddb)
    {
        _tableName = Env.GetString("TABLE_NAME");
        _ddb = ddb;
    }

    public async Task<PostEntity> CreatePostAsync(CreatePostArgs args)
    {
        var postId = Ulid.NewUlid().ToString();
        var pk = $"POST#{postId}";
        var sk = $"POST#{postId}";
        var now = DateTime.UtcNow;

        var entity = new PostEntity
        {
            Pk = pk,
            Sk = sk,
            PostId = postId,
            Title = args.Title,
            Body = args.Body,
            CreatedAt = now,
            UpdatedAt = now,
            Entity = "Post",
        };

        await _ddb.PutItemAsync(new PutItemRequest
        {
            TableName = _tableName,
            Item = entity.ToItem()
        });

        return entity;
    }

    public async Task<PostEntity?> GetPostAsync(string postId)
    {
        var pk = $"POST#{postId}";
        var sk = $"POST#{postId}";

        var response = await _ddb.GetItemAsync(new()
        {
            TableName = _tableName,
            Key = new()
            {
                [nameof(PostEntity.Pk)] = pk.ToAttr(),
                [nameof(PostEntity.Sk)] = sk.ToAttr(),
            }
        });

        return PostEntity.FromItem(response.Item);
    }

    public async Task<PostEntity?> UpdatePostAsync(string postId, UpdatePostArgs args)
    {
        var pk = $"POST#{postId}";
        var sk = $"POST#{postId}";
        var now = DateTime.UtcNow;

        try
        {
            var response = await _ddb.UpdateItemAsync(new UpdateItemRequest
            {
                TableName = _tableName,
                Key = new()
                {
                    [nameof(PostEntity.Pk)] = pk.ToAttr(),
                    [nameof(PostEntity.Sk)] = sk.ToAttr(),
                },
                UpdateExpression = "set #title = :title, #body = :body, #updatedAt = :updatedAt",
                ConditionExpression = "attribute_exists(id)",
                ExpressionAttributeValues = new()
                {
                    [":title"] = args.Title.ToAttr(),
                    [":body"] = args.Body.ToAttr(),
                    [":updatedAt"] = now.ToAttr(),
                },
                ExpressionAttributeNames = new()
                {
                    ["#title"] = nameof(PostEntity.Title),
                    ["#body"] = nameof(PostEntity.Body),
                    ["#updatedAt"] = nameof(PostEntity.UpdatedAt),
                },
                ReturnValues = ReturnValue.ALL_NEW,
            });

            return PostEntity.FromItem(response.Attributes);
        }
        catch (ConditionalCheckFailedException)
        {
            return null;
        }
    }

    public async Task<PostEntity?> DeletePostAsync(string postId)
    {
        var pk = $"POST#{postId}";
        var sk = $"POST#{postId}";

        try
        {
            var response = await _ddb.DeleteItemAsync(new()
            {
                TableName = _tableName,
                Key = new()
                {
                    [nameof(PostEntity.Pk)] = pk.ToAttr(),
                    [nameof(PostEntity.Sk)] = sk.ToAttr(),
                },
                ReturnValues = ReturnValue.ALL_OLD
            });

            return PostEntity.FromItem(response.Attributes);
        }
        catch (ConditionalCheckFailedException)
        {
            return null;
        }
    }
}