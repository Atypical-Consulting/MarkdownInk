using System.IO.Pipelines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using MarkdownInk.Mcp;
using MarkdownInk.Mcp.Configuration;
using MarkdownInk.Mcp.Services;
using MarkdownInk.Mcp.Tools;

namespace MarkdownInk.Mcp.Tests.Protocol;

/// <summary>
/// In-process MCP protocol test harness. Wires a real <c>McpServer</c> (built exactly as
/// <c>Program.cs</c> does — <c>AddMcpServer().WithToolsFromAssembly()</c>) to a real
/// <see cref="McpClient"/> over an in-memory duplex stream pair (two <see cref="Pipe"/>s), so tests
/// drive the same JSON-RPC pipeline (initialize, tools/list, tools/call, error envelopes) as the
/// shipped executable without spawning a process or touching stdio.
/// </summary>
internal sealed class McpProtocolTestHost : IAsyncDisposable
{
    private readonly IHost _host;

    public McpClient Client { get; }

    private McpProtocolTestHost(IHost host, McpClient client)
    {
        _host = host;
        Client = client;
    }

    public static async Task<McpProtocolTestHost> StartAsync()
    {
        // Two duplex pipes = four unidirectional streams, mirroring how stdio hooks a child
        // process's stdin/stdout together.
        var clientToServer = new Pipe();
        var serverToClient = new Pipe();

        var hostBuilder = new HostBuilder();
        hostBuilder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Warning);
        });
        hostBuilder.ConfigureServices(services =>
        {
            services
                .AddMcpServer(options => options.ServerInfo = MarkdownInkServerInfo.Create())
                .WithStreamServerTransport(clientToServer.Reader.AsStream(), serverToClient.Writer.AsStream())
                .WithToolsFromAssembly(typeof(RenderMarkdownTool).Assembly);

            // Mirror Program.cs so the tool schema tested here is identical to production.
            services.Configure<MarkdownInkMcpOptions>(_ => { });
            services.AddSingleton<IMarkdownRenderService, MarkdownRenderService>();
        });

        var host = hostBuilder.Build();
        await host.StartAsync();

        var clientTransport = new StreamClientTransport(
            serverInput: clientToServer.Writer.AsStream(),
            serverOutput: serverToClient.Reader.AsStream());

        var client = await McpClient.CreateAsync(clientTransport);

        return new McpProtocolTestHost(host, client);
    }

    public async ValueTask DisposeAsync()
    {
        await Client.DisposeAsync();
        await _host.StopAsync();
        _host.Dispose();
    }
}
