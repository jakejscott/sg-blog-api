﻿namespace SgBlogApi.Core;

public record InvalidRequest;
public record NotFound;
public record ValidationError(List<string> Errors);
public record ServerError;

public class ProblemDetailsResponse
{
    public int? StatusCode { get; set; }
    public string? ErrorCode { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class CreatePostRequest
{
    public string? Title { get; set; }
    public string? Body { get; set; }
}

public class CreatePostResponse
{
    public PostDto? Post { get; set; }
}

public class GetPostResponse
{
    public PostDto? Post { get; set; }
}

public class UpdatePostRequest
{
    public string? Title { get; set; }
    public string? Body { get; set; }
}

public class UpdatePostResponse
{
    public PostDto? Post { get; set; }
}