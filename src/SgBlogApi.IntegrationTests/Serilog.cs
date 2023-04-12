using Serilog;
using Xunit.Abstractions;

namespace SgBlogApi.IntegrationTests;

public static class SerilogXUnitConfiguration
{
    public static ILogger ConfigureLogging(ITestOutputHelper output)
    {
        if (output == null) throw new ArgumentNullException(nameof(output));

        var logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.TestOutput(output)
            .CreateLogger();

        return logger;
    }
}