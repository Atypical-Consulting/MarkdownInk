using Markdig.Renderers;
using Markdig.Syntax.Inlines;

namespace MarkdownInk.Rendering.Inlines;

internal class LineBreakInlineRenderer : MarkdownObjectRenderer<SpectreRenderer, LineBreakInline>
{
    protected override void Write(SpectreRenderer renderer, LineBreakInline obj)
    {
        if (obj.IsHard)
            renderer.InlineBuffer.Append('\n');
        else
            renderer.InlineBuffer.Append(' ');
    }
}
