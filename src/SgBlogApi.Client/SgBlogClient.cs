using System.Net;
using System.Text.Json;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;

namespace SgBlogApi.Client;

public interface ISgBlogClient
{
    Task<CreatePostResponse> CreatePostAsync(string blogId, CreatePostRequest request);

    Task<UpdatePostResponse> UpdatePostAsync(string blogId, string postId, UpdatePostRequest request);

    Task<DeletePostResponse> DeletePostAsync(string blogId, string postId);

    Task<GetPostResponse?> GetPostAsync(string blogId, string postId);

    Task<ListPostResponse> ListPostsAsync(string blogId, int limit = 25, string? paginationToken = null);
}

public class SgBlogClient : ISgBlogClient
{
    private readonly HttpClient _httpClient;
    
    private static AsyncRetryPolicy RetryPolicy()
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);

        return Policy.Handle<SgBlogClientException>(exception =>
            {
                return (int)exception.StatusCode >= 500 || exception.StatusCode switch
                {
                    HttpStatusCode.RequestTimeout => true,
                    HttpStatusCode.GatewayTimeout => true,
                    HttpStatusCode.NotFound => true,        // NOTE: Retry not found, for eventual consistency reasons.
                    HttpStatusCode.BadGateway => true,
                    HttpStatusCode.Unauthorized => true,    // NOTE: Retry due to Authorizer lambda issues.
                    HttpStatusCode.Forbidden => true,       // NOTE: Retry due to Authorizer lambda issues.
                    _ => false
                };
            })
            .WaitAndRetryAsync(delay);
    }

    public SgBlogClient(HttpClient httpClient, Uri serviceUrl)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = serviceUrl;
    }
    
    public async Task<CreatePostResponse> CreatePostAsync(string blogId, CreatePostRequest request)
    {
        return await RetryPolicy().ExecuteAsync(async () =>
        {
            var url = new Uri($"v1/{blogId}/posts", UriKind.Relative);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(request));

            using var httpResponse = await _httpClient.SendAsync(httpRequest);
            var content = await httpResponse.Content.ReadAsStringAsync();

            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                {
                    var result = JsonSerializer.Deserialize<CreatePostResponse>(content)!;

                    return result;
                }
                default:
                    throw new SgBlogClientException(httpResponse.StatusCode, content);
            }
        });
    }

    public async Task<UpdatePostResponse> UpdatePostAsync(string blogId, string postId, UpdatePostRequest request)
    {
        return await RetryPolicy().ExecuteAsync(async () =>
        {
            var url = new Uri($"v1/{blogId}/posts/{postId}", UriKind.Relative);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, url);
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(request));
            
            using var httpResponse = await _httpClient.SendAsync(httpRequest);
            var content = await httpResponse.Content.ReadAsStringAsync();

            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                {
                    var result = JsonSerializer.Deserialize<UpdatePostResponse>(content)!;

                    return result;
                }
                default:
                    throw new SgBlogClientException(httpResponse.StatusCode, content);
            }
        });
    }

    public async Task<DeletePostResponse> DeletePostAsync(string blogId, string postId)
    {
        return await RetryPolicy().ExecuteAsync(async () =>
        {
            var url = new Uri($"v1/{blogId}/posts/{postId}", UriKind.Relative);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Delete, url);

            using var httpResponse = await _httpClient.SendAsync(httpRequest);
            var content = await httpResponse.Content.ReadAsStringAsync();

            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                {
                    var result = JsonSerializer.Deserialize<DeletePostResponse>(content)!;

                    return result;
                }
                default:
                    throw new SgBlogClientException(httpResponse.StatusCode, content);
            }
        });
    }

    public async Task<GetPostResponse?> GetPostAsync(string blogId, string postId)
    {
        return await RetryPolicy().ExecuteAsync(async () =>
        {
            var url = new Uri($"v1/{blogId}/posts/{postId}", UriKind.Relative);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            
            using var httpResponse = await _httpClient.SendAsync(httpRequest);
            var content = await httpResponse.Content.ReadAsStringAsync();

            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                {
                    var result = JsonSerializer.Deserialize<GetPostResponse>(content)!;

                    return result;
                }
                case HttpStatusCode.NotFound:
                {
                    return null;
                }
                default:
                    throw new SgBlogClientException(httpResponse.StatusCode, content);
            }
        });
    }

    public async Task<ListPostResponse> ListPostsAsync(string blogId, int limit = 25, string? paginationToken = null)
    {
        return await RetryPolicy().ExecuteAsync(async () =>
        {
            var url = $"v1/{blogId}/posts?limit={limit}";
            if (paginationToken != null) url += $"&paginationToken={paginationToken}";

            using var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
            
            using var httpResponse = await _httpClient.SendAsync(httpRequest);
            var content = await httpResponse.Content.ReadAsStringAsync();

            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                {
                    var result = JsonSerializer.Deserialize<ListPostResponse>(content)!;

                    return result;
                }
                default:
                    throw new SgBlogClientException(httpResponse.StatusCode, content);
            }
        });
    }
}