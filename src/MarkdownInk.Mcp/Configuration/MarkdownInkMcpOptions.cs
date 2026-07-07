namespace MarkdownInk.Mcp.Configuration;

/// <summary>
/// Strongly-typed options bound from the "MarkdownInk" configuration section
/// (appsettings.json / appsettings.{Environment}.json / MARKDOWNINK_ environment variables).
/// </summary>
public class MarkdownInkMcpOptions
{
    /// <summary>
    /// The virtual terminal width (columns) used when a tool call does not specify one. Controls
    /// text wrapping and table layout.
    /// </summary>
    public int DefaultWidth { get; set; } = 100;

    /// <summary>
    /// Whether rendering emits ANSI color/style escape sequences by default when a tool call does
    /// not specify the <c>ansi</c> argument.
    /// </summary>
    public bool EnableColorByDefault { get; set; } = true;
}
