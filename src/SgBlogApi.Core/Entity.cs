namespace SgBlogApi.Core;

public class EntityBase
{
    public string? PK { get; set; }
    public string? SK { get; set; }
    public string? Entity { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PostEntity : EntityBase
{
    public string? PostId { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
}