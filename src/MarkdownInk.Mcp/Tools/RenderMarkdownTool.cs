using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using MarkdownInk.Mcp.Models;
using MarkdownInk.Mcp.Services;

namespace MarkdownInk.Mcp.Tools;

/// <summary>
/// MCP tool that renders inline Markdown text to terminal-ready output.
/// </summary>
[McpServerToolType]
public static class RenderMarkdownTool
{
    /// <summary>
    /// Renders a Markdown string to beautiful terminal output (ANSI by default).
    /// </summary>
    [McpServerTool(Title = "Render Markdown", ReadOnly = true, Destructive = false,
        Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Render a Markdown string to beautiful, terminal-ready output — headings, "
        + "syntax-highlighted code blocks, tables, lists, task lists, block quotes, footnotes, and "
        + "clickable links. Returns ANSI-colored text by default (set ansi:false for plain text). "
        + "Read-only: operates purely on the provided string and never touches the filesystem.")]
    public static ToolResult<RenderResponse> RenderMarkdown(
        IMarkdownRenderService renderService,
        [Description("The Markdown source to render.")]
        string markdown,
        [Description("Include ANSI color/style escape sequences for terminal display (default: true). "
            + "Set to false for plain text with the same layout but no color codes.")]
        bool ansi = true,
        [Description("Virtual terminal width in columns, used for wrapping and table layout (default: 100).")]
        int width = 100,
        ILoggerFactory? loggerFactory = null,
        CancellationToken cancellationToken = default)
    {
        using var invocation = ToolExecutionHelper.BeginInvocation(nameof(RenderMarkdown), loggerFactory);

        if (string.IsNullOrEmpty(markdown))
        {
            return ToolExecutionHelper.ValidationError<RenderResponse>(
                "The 'markdown' argument must be a non-empty string.",
                invocation.CorrelationId,
                "Pass the Markdown text to render, e.g. markdown: \"# Hello\".");
        }

        if (width <= 0)
        {
            return ToolExecutionHelper.ValidationError<RenderResponse>(
                "The 'width' argument must be a positive number of columns.",
                invocation.CorrelationId,
                "Use a value like 80 or 100.");
        }

        try
        {
            var output = renderService.Render(markdown, ansi, width);
            return ToolResult<RenderResponse>.Success(new RenderResponse
            {
                Output = output,
                Ansi = ansi,
                Width = width,
                LineCount = CountLines(output),
            });
        }
        catch (Exception ex)
        {
            return ToolExecutionHelper.Error<RenderResponse>(ex, invocation.CorrelationId, invocation.Logger);
        }
    }

    internal static int CountLines(string text)
    {
        if (text.Length == 0)
        {
            return 0;
        }

        var lines = 1;
        foreach (var c in text)
        {
            if (c == '\n')
            {
                lines++;
            }
        }

        // A trailing newline shouldn't count as an extra empty line.
        return text[^1] == '\n' ? lines - 1 : lines;
    }
}
