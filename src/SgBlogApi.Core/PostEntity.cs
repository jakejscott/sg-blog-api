using Amazon.DynamoDBv2.Model;

namespace SgBlogApi.Core;

public class PostEntity : EntityBase
{
    public required string BlogId { get; set; }
    public required string PostId { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }

    public static PostEntity Create(CreatePostArgs args)
    {
        var now = DateTime.UtcNow;
        var postId = Ulid.NewUlid().ToString();
        var pk = $"BLOG#{args.BlogId}";
        var sk = $"POST#{postId}";

        var entity = new PostEntity
        {
            Pk = pk,
            Sk = sk,
            BlogId = args.BlogId,
            PostId = postId,
            Title = args.Title,
            Body = args.Body,
            CreatedAt = now,
            UpdatedAt = now,
            Entity = "Post",
        };

        return entity;
    }

    public Dictionary<string, AttributeValue> ToItem()
    {
        return new()
        {
            [nameof(Pk)] = $"BLOG#{BlogId}".ToAttr(),
            [nameof(Sk)] = $"POST#{PostId}".ToAttr(),
            [nameof(Entity)] = "Post".ToAttr(),
            [nameof(BlogId)] = BlogId.ToAttr(),
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
            BlogId = item[nameof(BlogId)].GetString(),
            PostId = item[nameof(PostId)].GetString(),
            Title = item[nameof(Title)].GetString(),
            Body = item[nameof(Body)].GetString(),
            CreatedAt = item[nameof(CreatedAt)].GetDateTime(),
            UpdatedAt = item[nameof(UpdatedAt)].GetDateTime(),
        };
    }
}