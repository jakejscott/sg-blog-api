using System.Text.Json;
using System.Text.Json.Nodes;
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
    private readonly SerializerContext _serializerContext;
    private readonly IAmazonDynamoDB _ddb;
    private readonly string _tableName;

    public DynamoDbStore(IAmazonDynamoDB ddb, SerializerContext serializerContext)
    {
        _serializerContext = serializerContext;
        _tableName = Env.GetString("TABLE_NAME");
        _ddb = new AmazonDynamoDBClient();
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

        var jsonDocument = JsonSerializer.SerializeToDocument(entity, _serializerContext.PostEntity);
        var item = jsonDocument.ToItem();
        
        await _ddb.PutItemAsync(new PutItemRequest
        {
            TableName = _tableName,
            Item = item
        });

        return entity;
    }
}

public static class DynamoDbStoreExtensions
{
    public static JsonDocument FromItem(this Dictionary<string, AttributeValue> item)
    {
        throw new NotImplementedException();
    }

    public static Dictionary<string, AttributeValue> ToItem(this JsonDocument jsonDocument)
    {
        var item = new Dictionary<string, AttributeValue>();
        
        foreach (var jsonProperty in jsonDocument.RootElement.EnumerateObject())
        {
            var prop = JsonObject.Create(jsonProperty.Value)!;
            
            AttributeValue attr = prop.ToAttribute();
            
            // if (attr.Type == AttributeType.None) continue;
            
            item.Add(jsonProperty.Name, prop.ToAttribute());
        }

        return item;
    }
    
    private static AttributeValue ToAttribute(this JsonObject prop)
    {
        var ddbString = prop["S"];
        if (ddbString != null)
        {
            return new AttributeValue(ddbString.GetValue<string>());
        }
        
        var ddbNull = prop["NULL"];
        if (ddbNull != null)
        {
            return new AttributeValue { NULL = true };
        }
        
        var ddbBool = prop["BOOL"];
        if (ddbBool != null)
        {
            return new AttributeValue { BOOL = ddbBool.GetValue<bool>() };
        }

        var ddbNumber = prop["N"];
        if (ddbNumber != null)
        {
            return new AttributeValue { N = ddbNumber.GetValue<string>() };
        }

        throw new NotImplementedException();
    }
}