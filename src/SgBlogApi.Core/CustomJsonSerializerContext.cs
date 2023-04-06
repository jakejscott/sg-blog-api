using System.Text.Json.Serialization;
using Amazon.Lambda.APIGatewayEvents;

namespace SgBlogApi.Core;

[JsonSerializable(typeof(APIGatewayProxyRequest))]
[JsonSerializable(typeof(APIGatewayProxyResponse))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
// [JsonSerializable(typeof(Product))]
public partial class CustomJsonSerializerContext : JsonSerializerContext
{
    
}