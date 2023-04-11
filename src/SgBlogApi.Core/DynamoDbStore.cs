using System.Text;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace SgBlogApi.Core;

public class CreatePostArgs
{
    public required string BlogId { get; set; }
    public required string Title { get; init; }
    public required string Body { get; init; }
}

public class UpdatePostArgs
{
    public required string BlogId { get; set; }
    public required string PostId { get; set; }
    public required string Title { get; init; }
    public required string Body { get; init; }
}

public class DynamoDbStore
{
    private readonly IAmazonDynamoDB _ddb;
    private readonly SerializerContext _serializerContext;
    private readonly string _tableName;

    public DynamoDbStore(IAmazonDynamoDB ddb)
    {
        _tableName = Env.GetString("TABLE_NAME");
        _ddb = ddb;
        _serializerContext = new SerializerContext();
    }

    public async Task<PostEntity> CreatePostAsync(CreatePostArgs args)
    {
        var entity = PostEntity.Create(args);

        await _ddb.PutItemAsync(new PutItemRequest
        {
            TableName = _tableName,
            Item = entity.ToItem()
        });

        return entity;
    }

    public async Task<PostEntity?> GetPostAsync(string blogId, string postId)
    {
        var pk = $"BLOG#{blogId}";
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

        return response.IsItemSet ? PostEntity.FromItem(response.Item) : null;
    }

    public async Task<PostEntity?> UpdatePostAsync(UpdatePostArgs args)
    {
        var pk = $"BLOG#{args.BlogId}";
        var sk = $"POST#{args.PostId}";
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
                ConditionExpression = "attribute_exists(Pk) and attribute_exists(Sk)",
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

    public async Task<PostEntity?> DeletePostAsync(string blogId, string postId)
    {
        var pk = $"BLOG#{blogId}";
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

    public async Task<(List<PostEntity> Items, string? PaginationToken)> ListPostAsync(string blogId, int limit, string? paginationToken)
    {
        var response = await _ddb.QueryAsync(new QueryRequest
        {
            TableName = _tableName,
            KeyConditionExpression = "#pk = :pk",
            ExpressionAttributeValues = new()
            {
                [":pk"] = $"BLOG#{blogId}".ToAttr()
            },
            ExpressionAttributeNames = new()
            {
                ["#pk"] = "Pk"
            },
            Limit = limit,
            ScanIndexForward = false,
            ConsistentRead = false,
            Select = Select.ALL_ATTRIBUTES,
            ExclusiveStartKey = DecodeToken(paginationToken)
        });

        var entities = response.Items.Select(x => PostEntity.FromItem(x)!).ToList();
        var nextPaginationToken = EncodeToken(response);
        return (entities, nextPaginationToken);
    }

    private static string? EncodeToken(QueryResponse response)
    {
        if (response.LastEvaluatedKey is null || response.LastEvaluatedKey.Count <= 0)
            return null;
        var json = JsonSerializer.Serialize(response.LastEvaluatedKey);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }

    private Dictionary<string, AttributeValue>? DecodeToken(string? paginationToken)
    {
        if (paginationToken == null)
            return null;
        var json = Convert.FromBase64String(paginationToken);
        return JsonSerializer.Deserialize(json, _serializerContext.DictionaryStringAttributeValue)!;
    }
}