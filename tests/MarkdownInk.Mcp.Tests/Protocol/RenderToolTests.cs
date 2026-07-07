using System.Text.Json;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;
using Shouldly;

namespace MarkdownInk.Mcp.Tests.Protocol;

/// <summary>
/// One JSON-RPC <c>tools/call</c> round-trip per scenario, driven through a real <c>McpClient</c>
/// talking to an in-process <c>McpServer</c> (see <see cref="McpProtocolTestHost"/>). Exercises
/// argument (de)serialization, tool dispatch, and the MCP result/error envelope produced by the SDK.
/// </summary>
public class RenderToolTests
{
    private const char Esc = '\x1b';

    [Fact]
    public async Task RenderMarkdown_Ansi_Returns_WellFormed_Envelope_With_EscapeCodes()
    {
        await using var host = await McpProtocolTestHost.StartAsync();

        var result = await host.Client.CallToolAsync("render_markdown", new Dictionary<string, object?>
        {
            ["markdown"] = "# Hello **world**",
        });

        var data = AssertWellFormedSuccess(result);
        data.GetProperty("ansi").GetBoolean().ShouldBeTrue();
        var output = data.GetProperty("output").GetString();
        output.ShouldNotBeNullOrEmpty();
        output!.ShouldContain("Hello");
        // ANSI mode must emit escape sequences.
        output.ShouldContain(Esc);
        data.GetProperty("lineCount").GetInt32().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task RenderMarkdown_PlainText_Strips_EscapeCodes_But_Keeps_Content()
    {
        await using var host = await McpProtocolTestHost.StartAsync();

        var result = await host.Client.CallToolAsync("render_markdown", new Dictionary<string, object?>
        {
            ["markdown"] = "# Heading\n\nSome **bold** text.",
            ["ansi"] = false,
        });

        var data = AssertWellFormedSuccess(result);
        data.GetProperty("ansi").GetBoolean().ShouldBeFalse();
        var output = data.GetProperty("output").GetString();
        output.ShouldNotBeNullOrEmpty();
        output!.ShouldContain("Heading");
        output.ShouldContain("bold");
        // Plain-text mode must NOT emit escape sequences.
        output.ShouldNotContain(Esc);
    }

    [Fact]
    public async Task RenderMarkdown_Missing_Required_Markdown_Returns_McpLevel_Error()
    {
        await using var host = await McpProtocolTestHost.StartAsync();

        var result = await host.Client.CallToolAsync("render_markdown", new Dictionary<string, object?>());

        AssertMcpLevelError(result);
    }

    [Fact]
    public async Task RenderMarkdown_Empty_String_Returns_InBand_ValidationError()
    {
        await using var host = await McpProtocolTestHost.StartAsync();

        var result = await host.Client.CallToolAsync("render_markdown", new Dictionary<string, object?>
        {
            ["markdown"] = "",
        });

        // Empty (but present) input is a domain validation failure reported in-band: not an
        // MCP-level error, but ok:false in the envelope.
        result.IsError.ShouldNotBe(true);
        var root = ParseEnvelope(result);
        root.GetProperty("ok").GetBoolean().ShouldBeFalse();
        root.GetProperty("error").GetProperty("type").GetString().ShouldBe("ValidationError");
    }

    [Fact]
    public async Task RenderMarkdownFile_Missing_File_Returns_InBand_NotFound()
    {
        await using var host = await McpProtocolTestHost.StartAsync();

        var result = await host.Client.CallToolAsync("render_markdown_file", new Dictionary<string, object?>
        {
            ["path"] = "does-not-exist-1234.md",
        });

        result.IsError.ShouldNotBe(true);
        var root = ParseEnvelope(result);
        root.GetProperty("ok").GetBoolean().ShouldBeFalse();
        root.GetProperty("error").GetProperty("type").GetString().ShouldBe("NotFoundError");
    }

    [Fact]
    public async Task RenderMarkdownFile_Existing_File_Renders_And_Echoes_Path()
    {
        var path = Path.Combine(Path.GetTempPath(), $"mdink-mcp-test-{Guid.NewGuid():n}.md");
        await File.WriteAllTextAsync(path, "# From a file\n\n- one\n- two\n");
        try
        {
            await using var host = await McpProtocolTestHost.StartAsync();

            var result = await host.Client.CallToolAsync("render_markdown_file", new Dictionary<string, object?>
            {
                ["path"] = path,
                ["ansi"] = false,
            });

            var data = AssertWellFormedSuccess(result);
            data.GetProperty("path").GetString().ShouldBe(path);
            var output = data.GetProperty("output").GetString();
            output.ShouldNotBeNull();
            output!.ShouldContain("From a file");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public async Task Calling_Unknown_Tool_Surfaces_As_McpProtocolException_Not_A_Crash()
    {
        await using var host = await McpProtocolTestHost.StartAsync();

        var ex = await Should.ThrowAsync<McpProtocolException>(
            () => host.Client.CallToolAsync("totally_unknown_tool", new Dictionary<string, object?>()).AsTask());

        ex.Message.ShouldContain("totally_unknown_tool");

        // The session must still be usable after a protocol-level error.
        var tools = await host.Client.ListToolsAsync();
        tools.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task ListTools_Advertises_Both_Render_Tools()
    {
        await using var host = await McpProtocolTestHost.StartAsync();

        var tools = await host.Client.ListToolsAsync();
        var names = tools.Select(t => t.Name).ToList();

        names.ShouldContain("render_markdown");
        names.ShouldContain("render_markdown_file");
    }

    private static JsonElement ParseEnvelope(CallToolResult result)
    {
        var block = result.Content.ShouldHaveSingleItem();
        var text = block.ShouldBeOfType<TextContentBlock>();
        text.Text.ShouldNotBeNullOrWhiteSpace();
        return JsonDocument.Parse(text.Text).RootElement.Clone();
    }

    private static JsonElement AssertWellFormedSuccess(CallToolResult result)
    {
        result.IsError.ShouldNotBe(true);
        result.StructuredContent.ShouldNotBeNull();

        var root = ParseEnvelope(result);
        root.GetProperty("ok").GetBoolean().ShouldBeTrue();
        return root.GetProperty("data").Clone();
    }

    private static void AssertMcpLevelError(CallToolResult result)
    {
        result.IsError.ShouldBe(true);
        var block = result.Content.ShouldHaveSingleItem();
        var text = block.ShouldBeOfType<TextContentBlock>();
        text.Text.ShouldNotBeNullOrWhiteSpace();
    }
}
