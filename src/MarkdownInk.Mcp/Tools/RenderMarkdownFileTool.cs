using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using MarkdownInk.Mcp.Models;
using MarkdownInk.Mcp.Services;

namespace MarkdownInk.Mcp.Tools;

/// <summary>
/// MCP tool that renders a Markdown file (by path) to terminal-ready output.
/// </summary>
[McpServerToolType]
public static class RenderMarkdownFileTool
{
    /// <summary>
    /// Reads a Markdown file from disk and renders it to beautiful terminal output (ANSI by default).
    /// </summary>
    [McpServerTool(Title = "Render Markdown File", ReadOnly = true, Destructive = false,
        Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Read a Markdown file from disk and render it to beautiful, terminal-ready output — "
        + "headings, syntax-highlighted code blocks, tables, lists, task lists, block quotes, "
        + "footnotes, and clickable links. Returns ANSI-colored text by default (set ansi:false for "
        + "plain text). Read-only: reads the file but never modifies it.")]
    public static async Task<ToolResult<RenderResponse>> RenderMarkdownFile(
        IMarkdownRenderService renderService,
        [Description("Path to the Markdown file to render (absolute, or relative to the server's working directory).")]
        string path,
        [Description("Include ANSI color/style escape sequences for terminal display (default: true). "
            + "Set to false for plain text with the same layout but no color codes.")]
        bool ansi = true,
        [Description("Virtual terminal width in columns, used for wrapping and table layout (default: 100).")]
        int width = 100,
        ILoggerFactory? loggerFactory = null,
        CancellationToken cancellationToken = default)
    {
        using var invocation = ToolExecutionHelper.BeginInvocation(nameof(RenderMarkdownFile), loggerFactory);

        if (string.IsNullOrWhiteSpace(path))
        {
            return ToolExecutionHelper.ValidationError<RenderResponse>(
                "The 'path' argument must be a non-empty file path.",
                invocation.CorrelationId,
                "Pass the path to a Markdown file, e.g. path: \"README.md\".");
        }

        if (width <= 0)
        {
            return ToolExecutionHelper.ValidationError<RenderResponse>(
                "The 'width' argument must be a positive number of columns.",
                invocation.CorrelationId,
                "Use a value like 80 or 100.");
        }

        if (!File.Exists(path))
        {
            return ToolExecutionHelper.NotFound<RenderResponse>(
                $"File not found: {path}",
                invocation.CorrelationId,
                "Check the path. Relative paths resolve against the server's working directory.");
        }

        try
        {
            var markdown = await File.ReadAllTextAsync(path, cancellationToken);
            var output = renderService.Render(markdown, ansi, width);
            return ToolResult<RenderResponse>.Success(new RenderResponse
            {
                Output = output,
                Ansi = ansi,
                Width = width,
                LineCount = RenderMarkdownTool.CountLines(output),
                Path = path,
            });
        }
        catch (Exception ex)
        {
            return ToolExecutionHelper.Error<RenderResponse>(ex, invocation.CorrelationId, invocation.Logger);
        }
    }
}
