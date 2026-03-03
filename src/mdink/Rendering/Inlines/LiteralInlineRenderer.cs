using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using Spectre.Console;

namespace mdink.Rendering.Inlines;

internal class LiteralInlineRenderer : MarkdownObjectRenderer<SpectreRenderer, LiteralInline>
{
    protected override void Write(SpectreRenderer renderer, LiteralInline obj)
    {
        renderer.InlineBuffer.Append(Markup.Escape(obj.Content.ToString()));
    }
}
