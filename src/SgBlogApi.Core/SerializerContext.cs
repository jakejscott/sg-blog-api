using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.APIGatewayEvents;

namespace SgBlogApi.Core;

[JsonSerializable(typeof(CreatePostRequest))]
[JsonSerializable(typeof(CreatePostResponse))]
[JsonSerializable(typeof(UpdatePostRequest))]
[JsonSerializable(typeof(UpdatePostResponse))]
[JsonSerializable(typeof(GetPostResponse))]
[JsonSerializable(typeof(DeletePostResponse))]
[JsonSerializable(typeof(ListPostResponse))]
[JsonSerializable(typeof(PostDto))]
[JsonSerializable(typeof(PaginationToken))]
[JsonSerializable(typeof(APIGatewayProxyRequest))]
[JsonSerializable(typeof(APIGatewayProxyResponse))]
[JsonSerializable(typeof(ProblemDetailsResponse))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(Dictionary<string, AttributeValue>))]
public partial class SerializerContext : JsonSerializerContext
{
}