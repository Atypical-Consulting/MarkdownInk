using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MarkdownInk.Mcp;
using MarkdownInk.Mcp.Configuration;
using MarkdownInk.Mcp.Services;

try
{
    var host = CreateHostBuilder(args).Build();

    var logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("MarkdownInk MCP server starting...");
    logger.LogInformation("Process ID: {ProcessId}", Environment.ProcessId);
    logger.LogInformation("Runtime: {Runtime}", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);

    await host.RunAsync();

    logger.LogInformation("MarkdownInk MCP server stopped gracefully");
    return 0;
}
catch (Exception ex)
{
    // Fatal errors go to stderr — stdout is the JSON-RPC channel and must stay clean.
    await Console.Error.WriteLineAsync($"Fatal error: {ex.Message}");
    await Console.Error.WriteLineAsync($"Stack trace: {ex.StackTrace}");
    return 1;
}

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        // Anchor the content root to the directory the server binary lives in so the packaged
        // appsettings.json is loaded from the install location (AppContext.BaseDirectory), never
        // from whatever directory the process was launched in.
        .UseContentRoot(AppContext.BaseDirectory)
        .ConfigureHostConfiguration(host =>
        {
            // A one-shot stdio server never needs the FileSystemWatchers that reloadOnChange spins up.
            host.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["hostBuilder:reloadConfigOnChange"] = "false",
            });
        })
        .ConfigureAppConfiguration((_, config) =>
        {
            // appsettings.json is already added by CreateDefaultBuilder against the content root.
            // Only append MARKDOWNINK_-prefixed env vars and CLI args (last = highest precedence).
            config
                .AddEnvironmentVariables(prefix: "MARKDOWNINK_")
                .AddCommandLine(args);
        })
        .ConfigureServices((context, services) =>
        {
            services.Configure<MarkdownInkMcpOptions>(context.Configuration.GetSection("MarkdownInk"));

            services
                .AddMcpServer(options =>
                {
                    options.ServerInstructions = MarkdownInkToolGuidance.Instructions;
                    // Explicit serverInfo: the SDK default reports the MinVer-pinned {Major}.0.0.0
                    // AssemblyVersion; advertise the real package semver instead.
                    options.ServerInfo = MarkdownInkServerInfo.Create();
                })
                .WithStdioServerTransport()
                .WithToolsFromAssembly();

            // The rendering engine is stateless and thread-safe across calls (it builds a fresh
            // console + renderer per invocation), so a singleton is correct.
            services.AddSingleton<IMarkdownRenderService, MarkdownRenderService>();
        })
        .ConfigureLogging((context, logging) =>
        {
            logging.ClearProviders();

            // MCP stdio servers must emit ALL logs to stderr; stdout carries JSON-RPC.
            logging.AddConsole(options =>
            {
                options.LogToStandardErrorThreshold = LogLevel.Trace;
            });

            if (context.HostingEnvironment.IsDevelopment())
            {
                logging.SetMinimumLevel(LogLevel.Debug);
                logging.AddFilter("MarkdownInk", LogLevel.Debug);
            }
            else
            {
                logging.SetMinimumLevel(LogLevel.Information);
                logging.AddFilter("MarkdownInk", LogLevel.Information);
            }

            logging.AddFilter("Microsoft", LogLevel.Warning);
            logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information);
        })
        .UseConsoleLifetime(options => options.SuppressStatusMessages = true);

// Minimal Program class exposed for the test host / DI.
[ExcludeFromCodeCoverage]
public partial class Program
{
    static Program()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            var exception = e.ExceptionObject as Exception;
            Console.Error.WriteLine($"Unhandled exception: {exception?.Message ?? "Unknown error"}");
            if (exception != null)
            {
                Console.Error.WriteLine($"Stack trace: {exception.StackTrace}");
            }
            Environment.Exit(1);
        };

        TaskScheduler.UnobservedTaskException += (_, e) =>
        {
            e.SetObserved();
            foreach (var ex in e.Exception.Flatten().InnerExceptions)
            {
                Console.Error.WriteLine($"Unobserved task exception: {ex.Message}");
                Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        };
    }
}
