namespace SgBlogApi.Core;

public class EntityBase
{
    public required string PK { get; set; }
    public required string SK { get; set; }
    public required string Entity { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}

public class PostEntity : EntityBase
{
    public required string PostId { get; set; }
    public required string Title { get; set; }
    public required string Body { get; set; }
}