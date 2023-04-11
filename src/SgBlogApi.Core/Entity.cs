using EfficientDynamoDb.Attributes;

namespace SgBlogApi.Core;

[DynamoDbTable("blog")]
public class EntityBase
{
    [DynamoDbProperty(nameof(PK), DynamoDbAttributeType.PartitionKey)]
    public required string PK { get; set; }

    [DynamoDbProperty(nameof(SK), DynamoDbAttributeType.SortKey)]
    public required string SK { get; set; }
        
    [DynamoDbProperty(nameof(Entity))]
    public required string Entity { get; set; }
    
    [DynamoDbProperty(nameof(CreatedAt))]
    public required DateTime CreatedAt { get; set; }

    [DynamoDbProperty(nameof(UpdatedAt))]
    public required DateTime UpdatedAt { get; set; }
}

public class PostEntity : EntityBase
{
    [DynamoDbProperty(nameof(PostId))]
    public required string PostId { get; set; }
    
    [DynamoDbProperty(nameof(Title))]
    public required string Title { get; set; }
    
    [DynamoDbProperty(nameof(Body))]
    public required string Body { get; set; }
}