namespace SgBlogApi.Core;

public class PostDto
{
    public string? BlogId { get; set; }
    public string? PostId { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}