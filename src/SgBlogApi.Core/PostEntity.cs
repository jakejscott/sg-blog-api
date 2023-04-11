using Amazon.DynamoDBv2.Model;

namespace SgBlogApi.Core;

public class PostEntity : EntityBase
{
    public required string PostId { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }

    public Dictionary<string, AttributeValue> ToItem()
    {
        return new()
        {
            [nameof(Pk)] = $"POST#{PostId}".ToAttr(),
            [nameof(Sk)] = $"POST#{PostId}".ToAttr(),
            [nameof(Entity)] = "Post".ToAttr(),
            [nameof(PostId)] = PostId.ToAttr(),
            [nameof(Title)] = Title.ToAttr(),
            [nameof(Body)] = Body.ToAttr(),
            [nameof(CreatedAt)] = CreatedAt.ToAttr(),
            [nameof(UpdatedAt)] = CreatedAt.ToAttr(),
        };
    }

    public static PostEntity? FromItem(Dictionary<string, AttributeValue>? item)
    {
        if (item is null) return null;
        if (item.Count == 0) return null;

        return new()
        {
            Pk = item[nameof(Pk)].GetString(),
            Sk = item[nameof(Sk)].GetString(),
            Entity = item[nameof(Entity)].GetString(),
            PostId = item[nameof(PostId)].GetString(),
            Title = item[nameof(Title)].GetString(),
            Body = item[nameof(Body)].GetString(),
            CreatedAt = item[nameof(CreatedAt)].GetDateTime(),
            UpdatedAt = item[nameof(UpdatedAt)].GetDateTime(),
        };
    }
}