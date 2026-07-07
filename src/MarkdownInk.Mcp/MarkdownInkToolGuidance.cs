namespace MarkdownInk.Mcp;

/// <summary>
/// Server-level MCP <c>instructions</c> sent to the client on initialize. This is the one message
/// the model reliably sees at tool-selection time, so it states concretely what the tools do and
/// when to reach for them.
/// </summary>
internal static class MarkdownInkToolGuidance
{
    public const string Instructions =
        """
        MarkdownInk renders Markdown into beautiful, terminal-ready output — the same engine as the
        `mdink` CLI (Markdig + Spectre.Console + TextMateSharp syntax highlighting). Reach for these
        tools whenever you want to *display* Markdown to a user in a terminal, rather than emit raw
        Markdown source:

        - Render Markdown you already have as text → `render_markdown` (pass the string).
        - Render a Markdown file on disk → `render_markdown_file` (pass its path).

        Both return ANSI-colored output by default (headings, syntax-highlighted fenced code blocks,
        tables, ordered/unordered/task lists, nested block quotes, footnotes, and OSC-8 clickable
        links). Set `ansi: false` to get the same layout as plain text with no color escape codes —
        useful when the consumer can't interpret ANSI. Use `width` (default 100) to control wrapping
        and table sizing for the target terminal.

        These tools only render; they never write to disk. `render_markdown_file` reads the file but
        does not modify it.
        """;
}
