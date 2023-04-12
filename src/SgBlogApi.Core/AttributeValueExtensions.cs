using System.Globalization;
using Amazon.DynamoDBv2.Model;

namespace SgBlogApi.Core;

public static class AttributeValueExtensions
{
    public static AttributeValue ToAttr(this string? value)
    {
        return value == null ? new AttributeValue { NULL = true } : new AttributeValue(value);
    }

    public static AttributeValue ToAttr(this DateTime value)
    {
        return new AttributeValue { S = value.ToString("O", CultureInfo.InvariantCulture) };
    }

    public static string GetString(this AttributeValue value)
    {
        return value.S;
    }

    public static DateTime GetDateTime(this AttributeValue value)
    {
        if (!DateTime.TryParseExact(value.S, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dateTime))
        {
            throw new Exception($"Couldn't parse DateTime ddb value from '{value.S}'");
        }
        return dateTime;
    }
}