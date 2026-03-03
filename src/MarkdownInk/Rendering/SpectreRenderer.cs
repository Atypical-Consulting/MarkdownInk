using System.Text;
using Markdig.Renderers;
using Markdig.Syntax;
using Spectre.Console;
using MarkdownInk.Rendering.Blocks;
using MarkdownInk.Rendering.Inlines;

namespace MarkdownInk.Rendering;

public class SpectreRenderer : RendererBase
{
    internal readonly StringBuilder InlineBuffer = new();
    internal readonly Lazy<SyntaxHighlighter> Highlighter = new(() => new SyntaxHighlighter());
    internal int IndentLevel;
    internal string? QuotePrefix;
    internal IAnsiConsole Console { get; }

    public SpectreRenderer(IAnsiConsole? console = null)
    {
        Console = console ?? AnsiConsole.Console;

        // Block renderers
        ObjectRenderers.Add(new HeadingRenderer());
        ObjectRenderers.Add(new ParagraphRenderer());
        ObjectRenderers.Add(new CodeBlockRenderer());
        ObjectRenderers.Add(new ThematicBreakRenderer());
        ObjectRenderers.Add(new QuoteBlockRenderer());
        ObjectRenderers.Add(new ListRenderer());
        ObjectRenderers.Add(new TableRenderer());
        ObjectRenderers.Add(new FootnoteGroupRenderer());

        // Inline renderers
        ObjectRenderers.Add(new LiteralInlineRenderer());
        ObjectRenderers.Add(new EmphasisInlineRenderer());
        ObjectRenderers.Add(new CodeInlineRenderer());
        ObjectRenderers.Add(new LinkInlineRenderer());
        ObjectRenderers.Add(new LineBreakInlineRenderer());
        ObjectRenderers.Add(new TaskListRenderer());
        ObjectRenderers.Add(new FootnoteLinkRenderer());
    }

    public override object Render(MarkdownObject markdownObject)
    {
        Write(markdownObject);
        return this;
    }

    internal string DrainInlineBuffer()
    {
        var text = InlineBuffer.ToString();
        InlineBuffer.Clear();
        return text;
    }

    internal void FlushInline()
    {
        var text = DrainInlineBuffer();
        if (string.IsNullOrWhiteSpace(text)) return;

        var indent = new string(' ', IndentLevel * 2);
        var prefix = QuotePrefix ?? "";
        Console.Markup($"{indent}{prefix}{text}\n");
    }
}
