using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using Spectre.Console;

namespace MarkdownInk.Rendering.Inlines;

internal class LiteralInlineRenderer : MarkdownObjectRenderer<SpectreRenderer, LiteralInline>
{
    protected override void Write(SpectreRenderer renderer, LiteralInline obj)
    {
        renderer.InlineBuffer.Append(Markup.Escape(obj.Content.ToString()));
    }
}
