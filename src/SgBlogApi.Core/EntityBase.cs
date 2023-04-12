namespace SgBlogApi.Core;

public class EntityBase
{
    public required string Pk { get; set; }
    public required string Sk { get; set; }
    public required string Entity { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}