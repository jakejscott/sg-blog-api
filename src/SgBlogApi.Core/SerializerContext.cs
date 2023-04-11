using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;

namespace SgBlogApi.Core;

[JsonSerializable(typeof(CreatePostRequest))]
[JsonSerializable(typeof(CreatePostResponse))]
[JsonSerializable(typeof(UpdatePostRequest))]
[JsonSerializable(typeof(UpdatePostResponse))]
[JsonSerializable(typeof(GetPostResponse))]
[JsonSerializable(typeof(PostDto))]
[JsonSerializable(typeof(APIGatewayProxyRequest))]
[JsonSerializable(typeof(APIGatewayProxyResponse))]
[JsonSerializable(typeof(ProblemDetailsResponse))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
public partial class SerializerContext : JsonSerializerContext
{
}