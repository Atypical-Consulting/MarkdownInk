namespace MarkdownInk.Mcp.Services;

/// <summary>
/// Renders Markdown to terminal-ready text using the MarkdownInk engine (Markdig parse +
/// <c>SpectreRenderer</c>), capturing the output as a string instead of writing to a live terminal.
/// </summary>
public interface IMarkdownRenderService
{
    /// <summary>
    /// Renders <paramref name="markdown"/> to a string.
    /// </summary>
    /// <param name="markdown">The Markdown source to render.</param>
    /// <param name="ansi">
    /// When <see langword="true"/>, the output contains ANSI color/style escape sequences (the
    /// "beautiful" terminal rendering). When <see langword="false"/>, the same structural layout is
    /// produced with all color escapes stripped — cleaner for plain-text consumption.
    /// </param>
    /// <param name="width">The virtual terminal width, in columns, used for wrapping and table layout.</param>
    /// <returns>The rendered output.</returns>
    string Render(string markdown, bool ansi, int width);
}
