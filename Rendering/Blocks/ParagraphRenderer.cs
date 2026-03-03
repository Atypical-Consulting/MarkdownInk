using Markdig.Renderers;
using Markdig.Syntax;
using Spectre.Console;

namespace mdink.Rendering.Blocks;

internal class ParagraphRenderer : MarkdownObjectRenderer<SpectreRenderer, ParagraphBlock>
{
    protected override void Write(SpectreRenderer renderer, ParagraphBlock obj)
    {
        renderer.WriteChildren(obj.Inline!);
        renderer.FlushInline();
        AnsiConsole.WriteLine();
    }
}
