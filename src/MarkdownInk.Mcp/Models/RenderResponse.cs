using System.Text.Json.Serialization;

namespace MarkdownInk.Mcp.Models;

/// <summary>
/// The success payload returned by the render tools: the rendered terminal output plus the
/// settings it was produced with.
/// </summary>
public sealed class RenderResponse
{
    /// <summary>The rendered output. Contains ANSI escape sequences when <see cref="Ansi"/> is true.</summary>
    [JsonPropertyName("output")]
    public string Output { get; init; } = string.Empty;

    /// <summary>Whether the output includes ANSI color/style escape sequences.</summary>
    [JsonPropertyName("ansi")]
    public bool Ansi { get; init; }

    /// <summary>The virtual terminal width (columns) used for wrapping and table layout.</summary>
    [JsonPropertyName("width")]
    public int Width { get; init; }

    /// <summary>The number of lines in <see cref="Output"/>.</summary>
    [JsonPropertyName("lineCount")]
    public int LineCount { get; init; }

    /// <summary>The source file that was rendered; omitted for inline (string) rendering.</summary>
    [JsonPropertyName("path")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Path { get; init; }
}
