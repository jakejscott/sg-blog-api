using System.Net;

namespace SgBlogApi.Client;

public class SgBlogClientException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public string Content { get; }
        
    public SgBlogClientException(HttpStatusCode statusCode, string content) : base(FormatMessage(statusCode, content))
    {
        StatusCode = statusCode;
        Content = content;
    }

    private static string FormatMessage(HttpStatusCode statusCode, string content)
    {
        return $"StatusCode: {statusCode} Response:\n{content}";
    }
}