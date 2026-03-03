using Markdig.Renderers;
using Markdig.Syntax;
using Spectre.Console;

namespace MarkdownInk.Rendering.Blocks;

internal class ParagraphRenderer : MarkdownObjectRenderer<SpectreRenderer, ParagraphBlock>
{
    protected override void Write(SpectreRenderer renderer, ParagraphBlock obj)
    {
        renderer.WriteChildren(obj.Inline!);
        renderer.FlushInline();
        renderer.Console.WriteLine();
    }
}
