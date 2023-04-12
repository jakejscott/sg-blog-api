using Riok.Mapperly.Abstractions;

namespace SgBlogApi.Core;

[Mapper]
public partial class PostMapper
{
    public partial PostDto PostToDto(PostEntity entity);
}