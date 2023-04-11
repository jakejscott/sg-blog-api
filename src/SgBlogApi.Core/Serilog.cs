using Serilog;
using Serilog.Formatting.Json;

namespace SgBlogApi.Core;

public static class SerilogConfiguration
{
    public static ILogger ConfigureLogging()
    {
        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console(new JsonFormatter())
            .CreateLogger();

        return logger;
    }
}