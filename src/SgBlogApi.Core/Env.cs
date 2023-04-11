namespace SgBlogApi.Core;

public static class Env
{
    public static string GetString(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        if (string.IsNullOrWhiteSpace(value)) throw new Exception($"Env var {key} was missing");
        return value;
    }
}